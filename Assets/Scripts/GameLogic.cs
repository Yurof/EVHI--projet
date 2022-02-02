using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class GameLogic : MonoBehaviour
{
    public AudioSource hitaudio;
    public AudioSource missaudio;

    public Target[] targets;

    public float spawnTimer;

    public Score score;
    public Timer timer;
    public GameUI ui;
    public RandomLocation location;

    private int currentTargetsOnScreen;
    private int points;
    private int pointsCombo;

    //cibles désactivées
    private List<Target> disabledTargets = new List<Target>();

    //temps d'attente entre les cibles
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
        foreach (Target m in targets)
        {
            m.OnTargetDied += TargetDied;
        }

        timer.OnTimeOut += GameOver;

        wait = new WaitForSeconds(spawnTimer);
    }

    //Lorsque une cible disparait, qu'on est cliqué dessus ou non
    private void TargetDied(Target target, bool clicked)
    {
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
            targetSize = Math.Max(targetSize * 0.9f, targetMinSize);
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
                targetSize = Math.Min(targetSize * 1.5f, targetMaxSize);
            }
        }
    }

    public void NewGame()
    {
        ui.NewGame();
        score.NewGame();
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

    private IEnumerator SpawnTargets()
    {
        while (true)
        {
            if (currentTargetsOnScreen == 0 && disabledTargets.Count > 0)
            {
                yield return wait;
                disabledTargets[0].Respawn(location.FindPosition(fittDist), setTargetData());
                disabledTargets.RemoveAt(0);
                currentTargetsOnScreen++;
            }

            yield return null;
        }
    }

    private TargetData setTargetData()
    {
        TargetData T = ScriptableObject.CreateInstance<TargetData>();
        T.size = targetSize;
        T.timeOnScreen = targetDuration;
        return T;
    }

    public void ResetCombo()
    {
        missaudio.Play();
        score.UpdateAccurcy(false);
        animatorCombo.SetTrigger("breakerTrigger");
        pointsCombo = 0;
        targetSize = Math.Min(targetSize * 1.2f, targetMaxSize);
        score.UpdateCombo(pointsCombo);
    }
}