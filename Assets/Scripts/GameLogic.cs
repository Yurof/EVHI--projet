using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class GameLogic : MonoBehaviour
{
    public AudioSource hitaudio;
    public AudioSource missaudio;

    /// The targets in the scene.
    public Target[] targets;

    /// The time interval to spawn a new target.
    public float spawnTimer;

    /// Score handles the score and display it.
    public Score score;

    /// Timer handels the game round time and riase an event when time is up to indicate the end of the round.
    public Timer timer;

    /// Changes the game ui state from "gameplay" to "menu" and viseversa.
    public GameUI ui;

    /// Location is responable for choosing a random location to spawn the target at.
    public RandomLocation location;

    /// The number of targets currently on screen.
    private int currentTargetsOnScreen;

    /// The current score of an active game round.
    private int points;

    private int pointsCombo;

    /// All the disabled target. we use one of them when we need to spawn a new target.
    private List<Target> disabledTargets = new List<Target>();

    /// Wait time used to spawning coroutine.
    private WaitForSeconds wait;

    public Animator animatorCombo;

    private List<float> timeKillList;

    private float oldMeanTimeKill;
    private float meanTimeKill;
    private float fittDist;
    private float a = 0f;
    private float b = 0.13f;

    public float targetMinSize = 0.5f;
    public float targetMaxSize = 2f;
    public float targetMinDuration = 0.2f;
    public float targetMaxDuration = 3f;

    public float targetSize = 1.5f;
    public float targetDuration = 2f;

    private System.Random rnd = new System.Random();

    public bool verbose = false;

    private void Awake()
    {
        // Listen to all the targets' click event.
        foreach (Target m in targets)
        {
            m.OnTargetDied += TargetDied;
        }

        // liseten to the timer's timeout event.
        timer.OnTimeOut += GameOver;

        wait = new WaitForSeconds(spawnTimer);
    }

    /// Call back when a target was clicked.
    /// if clicked is true. the target was actually click and we need to add to the score.
    private void TargetDied(Target target, bool clicked)
    {
        location.FreeLocation(target);
        disabledTargets.Add(target);
        currentTargetsOnScreen--;

        if (clicked)
        {
            score.UpdateAccurcy(true);
            hitaudio.Play();
            points += 1;
            pointsCombo += 1;
            score.UpdateScore(points);
            score.UpdateCombo(pointsCombo);
            animatorCombo.SetTrigger("comboTrigger");

            timeKillList.Add(Time.time - target.data.spawnTime);
            oldMeanTimeKill = meanTimeKill;
            meanTimeKill = Queryable.Average(timeKillList.AsQueryable());

            fittDist = targetSize * ((float)Math.Pow(2, ((3 / meanTimeKill) - a) / b) - 1);

            if (meanTimeKill - oldMeanTimeKill <= 0)
            {
                if (verbose)
                {
                    Debug.Log("Target duration decreased");
                }
                targetDuration = Math.Max(targetDuration * 0.9f, targetMinDuration);
            }
            else
            {
                if (verbose)
                {
                    Debug.Log("Target duration increased");
                }
                targetDuration = Math.Min(targetDuration * 1.1f, targetMaxDuration);
            }

            if (verbose)
            {
                Debug.Log("Target size decreased");
            }
            targetSize = Math.Max(targetSize * 0.97f, targetMinSize);
        }
        else
        {
            ResetCombo();
            timeKillList.Add(targetMaxDuration);
            oldMeanTimeKill = meanTimeKill;
            meanTimeKill = Queryable.Average(timeKillList.AsQueryable());

            if (rnd.Next(2) == 1)
            {
                if (verbose)
                {
                    Debug.Log("Target duration increased");
                }
                targetDuration = Math.Min(targetDuration * 1.1f, targetMaxDuration);
            }
            else
            {
                if (verbose)
                {
                    Debug.Log("Target size increased");
                }
                targetSize = Math.Min(targetSize * 1.8f, targetMaxSize);
            }
        }
    }

    public void NewGame()
    {
        ui.NewGame();
        score.NewGame();
        location.NewGame();
        disabledTargets.Clear();

        if (targetSize <= 0.8f && targetDuration <= 1.2f)
        {
            ui.ChangeBackground();
        }

        foreach (Target m in targets)
        {
            m.Despawn();
            disabledTargets.Add(m);
        }

        points = 0;
        currentTargetsOnScreen = 0;

        timeKillList = new List<float>();

        oldMeanTimeKill = 0f;
        meanTimeKill = 0f;

        StartCoroutine("SpawnTargets");
        //SpawnImmediate();
        timer.NewGame();
    }

    private void GameOver()
    {
        StopCoroutine("SpawnTargets");
        score.scoremeanTimeKill = meanTimeKill;
        ui.GameOver();
        score.GameOver(points);
        pointsCombo = 0;
        score.UpdateCombo(pointsCombo);
    }

    /// Coroutine to spawn a new target every time interval.
    private IEnumerator SpawnTargets()
    {
        while (true)
        {
            if (currentTargetsOnScreen == 0 && disabledTargets.Count > 0)
            {
                yield return wait;
                disabledTargets[0].Respawn(location.FindLocation(disabledTargets[0], fittDist), setTargetData());
                disabledTargets.RemoveAt(0);
                currentTargetsOnScreen++;
            }

            yield return null;
        }
    }

    private TargetData setTargetData()
    {
        return new TargetData { size = targetSize, timeOnScreen = targetDuration };
    }

    public void ResetCombo()
    {
        missaudio.Play();
        score.UpdateAccurcy(false);
        animatorCombo.SetTrigger("breakerTrigger");
        pointsCombo = 0;
        score.UpdateCombo(pointsCombo);
    }
}