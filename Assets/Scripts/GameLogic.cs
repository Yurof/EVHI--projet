﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class GameLogic : MonoBehaviour
{
    public AudioSource hitaudio;

    /// The moles in the scene.
    public Mole[] moles;

    /// All the mole types, MoleData is a scriptable object.
    public MoleData[] moleData;

    /// The time interval to spawn a new mole.
    public float spawnTimer;

    /// Score handles the score and display it.
    public Score score;

    /// Timer handels the game round time and riase an event when time is up to indicate the end of the round.
    public Timer timer;

    /// Changes the game ui state from "gameplay" to "menu" and viseversa.
    public GameUI ui;

    /// Location is responable for choosing a random location to spawn the mole at.
    public RandomLocation location;

    /// The number of moles currently on screen.
    private int currentMolesOnScreen;

    /// The current score of an active game round.
    private int points;

    private int pointsCombo;

    /// All the disabled mole. we use one of them when we need to spawn a new mole.
    private List<Mole> disabledMoles = new List<Mole>();

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
        // Listen to all the moles' click event.
        foreach (Mole m in moles)
        {
            m.OnMoleDied += MoleDied;
        }

        // liseten to the timer's timeout event.
        timer.OnTimeOut += GameOver;

        wait = new WaitForSeconds(spawnTimer);
    }

    /// Call back when a mole was clicked.
    /// if clicked is true. the mole was actually click and we need to add to the score.
    private void MoleDied(Mole mole, bool clicked)
    {
        location.FreeLocation(mole);
        disabledMoles.Add(mole);
        currentMolesOnScreen--;
        Debug.Log("click?" + clicked);

        if (clicked)
        {
            score.UpdateAccurcy(true);
            hitaudio.Play();
            points += 1;
            pointsCombo += 1;
            score.UpdateScore(points);
            score.UpdateCombo(pointsCombo);
            animatorCombo.SetTrigger("comboTrigger");

            timeKillList.Add(Time.time - mole.data.spawnTime);
            oldMeanTimeKill = meanTimeKill;
            meanTimeKill = Queryable.Average(timeKillList.AsQueryable());
            Debug.Log("meanTimeKill" + meanTimeKill);

            fittDist = targetSize * ((float)Math.Pow(2, ((1 / meanTimeKill) - a) / b) - 1);
            Debug.Log("fitz " + meanTimeKill + " " + a + " " + b + " " + fittDist);

            /*
                        if (verbose)
                        {
                            Debug.Log("--------------------");
                            Debug.Log("Moment d'apparition de la cible : " + mole.data.spawnTime);
                            Debug.Log("Moment de destruction de la cible : " + Time.time);
                            Debug.Log("timeToKill : " + (Time.time - mole.data.spawnTime));

                            for (int i = 0; i < timeKillList.Count; i++)
                            {
                                Debug.Log("--------------------");
                                Debug.Log("Moment d'apparition de la cible : " + mole.data.spawnTime);
                                Debug.Log("Moment de destruction de la cible : " + Time.time);
                                Debug.Log("timeToKill : " + (Time.time - mole.data.spawnTime));

                            Debug.Log("Valeur de meanTimeKill : " + meanTimeKill);
                        }*/

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
                targetSize = Math.Min(targetSize * 1.1f, targetMaxSize);
            }
        }
    }

    public void NewGame()
    {
        ui.NewGame();
        score.NewGame();
        location.NewGame();
        disabledMoles.Clear();

        Debug.Log("targetDuration" + targetDuration);
        Debug.Log("targetSize" + targetSize);

        foreach (Mole m in moles)
        {
            m.Despawn();
            disabledMoles.Add(m);
        }

        points = 0;
        currentMolesOnScreen = 0;

        timeKillList = new List<float>();

        oldMeanTimeKill = 0f;
        meanTimeKill = 0f;

        StartCoroutine("SpawnMoles");
        SpawnImmediate();
        timer.NewGame();
    }

    private void GameOver()
    {
        StopCoroutine("SpawnMoles");
        score.scoremeanTimeKill = meanTimeKill;
        ui.GameOver();
        score.GameOver(points);
        pointsCombo = 0;
        score.UpdateCombo(pointsCombo);
    }

    /// Coroutine to spawn a new mole every time interval.
    private IEnumerator SpawnMoles()
    {
        while (true)
        {
            if (currentMolesOnScreen == 0 && disabledMoles.Count > 0)
            {
                disabledMoles[0].Respawn(location.FindLocation(disabledMoles[0], fittDist), RandomMole());
                disabledMoles.RemoveAt(0);
                currentMolesOnScreen++;
            }

            yield return wait;
        }
    }

    /// Spawn moles immediatly to make sure at least 1 moles are on screen.
    private void SpawnImmediate()
    {
        while (currentMolesOnScreen < 1 && disabledMoles.Count > 0)
        {
            disabledMoles[0].Respawn(location.FindLocation(disabledMoles[0], fittDist), RandomMole());
            disabledMoles.RemoveAt(0);
            currentMolesOnScreen++;
        }
    }

    /// Picks a random scriptable object for the mole's data.
    private MoleData RandomMole()
    {
        return new MoleData { size = targetSize, timeOnScreen = targetDuration };
    }

    public void ResetCombo()
    {
        score.UpdateAccurcy(false);
        animatorCombo.SetTrigger("breakerTrigger");
        pointsCombo = 0;
        score.UpdateCombo(pointsCombo);
    }
}