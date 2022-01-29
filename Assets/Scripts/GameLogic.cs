﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


public class GameLogic : MonoBehaviour
{
    /// <summary>
    /// The moles in the scene.
    /// </summary>
    public Mole[] moles;

    /// <summary>
    /// All the mole types, MoleData is a scriptable object.
    /// </summary>
    public MoleData[] moleData;

    /// <summary>
    /// The time interval to spawn a new mole.
    /// </summary>
    public float spawnTimer;

    /// <summary>
    /// Score handles the score and display it.
    /// </summary>
    public Score score;

    /// <summary>
    /// Timer handels the game round time and riase an event when time is up to indicate the end of the round.
    /// </summary>
    public Timer timer;

    /// <summary>
    /// Changes the game ui state from "gameplay" to "menu" and viseversa.
    /// </summary>
    public GameUI ui;

    /// <summary>
    /// Location is responable for choosing a random location to spawn the mole at.
    /// </summary>
    public RandomLocation location;

    /// <summary>
    /// The number of moles currently on screen.
    /// </summary>
    private int currentMolesOnScreen;

    /// <summary>
    /// The current score of an active game round.
    /// </summary>
    private int points;

    private int pointsCombo;

    /// <summary>
    /// All the disabled mole. we use one of them when we need to spawn a new mole.
    /// </summary>
    private List<Mole> disabledMoles = new List<Mole>();

    /// <summary>
    /// Wait time used to spawning coroutine.
    /// </summary>
    private WaitForSeconds wait;

    public Animator animatorCombo;

    private List<float> timeKillList;

    private float meanTimeKill;


    private float targetMinSize;
    private float targetMaxSize;
    private float targetMinDuration;
    private float targetMaxDuration;

    private float targetSize;
    private float targetDuration;

    private System.Random rnd = new System.Random();

    public bool verbose = true;



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

    /// <summary>
    /// Call back when a mole was clicked.
    /// if clicked is true. the mole was actually click and we need to add to the score.
    /// </summary>
    /// <param name="mole"></param>
    /// <param name="clicked"></param>
    /// 
    /* Increase or decrease either duration or size of the targets whether target was clicked on*/
    private void MoleDied(Mole mole, bool clicked)
    {
        location.FreeLocation(mole);
        disabledMoles.Add(mole);
        currentMolesOnScreen--;

        if (clicked)
        {
            points += 1;
            pointsCombo += 1;
            score.UpdateScore(points);
            score.UpdateCombo(pointsCombo);
            animatorCombo.SetTrigger("comboTrigger");
            score.UpdateAccurcy(true);

            
            timeKillList.RemoveAt(0);
            timeKillList.Add(Time.time - mole.data.spawnTime);
            meanTimeKill = Queryable.Average(timeKillList.AsQueryable()); ;
/*
            if (verbose)
            {
                Debug.Log("--------------------");
                Debug.Log("Moment d'apparition de la cible : " + mole.data.spawnTime);
                Debug.Log("Moment de destruction de la cible : " + Time.time);
                Debug.Log("timeToKill : " + (Time.time - mole.data.spawnTime));

                for (int i = 0; i < timeKillList.Count; i++)
                {
                    Debug.Log("Valeur de timeKillList[" + i + "] : " + timeKillList[i]);
                }

                Debug.Log("Valeur de meanTimeKill : " + meanTimeKill);
            }*/
            if (rnd.Next(2) == 1)
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
                    Debug.Log("Target size decreased");
                }
                targetSize = Math.Max(targetSize * 0.9f, targetMinSize);
            }
            
        }
        else
        {
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

        SpawnImmediate();
    }

    public void NewGame()
    {
        ui.NewGame();
        score.NewGame();
        location.NewGame();
        disabledMoles.Clear();

        foreach (Mole m in moles)
        {
            m.Despawn();
            disabledMoles.Add(m);
        }

        points = 0;
        currentMolesOnScreen = 0;

        timeKillList = new List<float>();
        for (int i = 0; i < 10; ++i)
        {
            timeKillList.Add(0f);
        }

        meanTimeKill = 0f;


        targetMinSize = 1f;
        targetMaxSize = 2f;
        targetMinDuration = 1f;
        targetMaxDuration = 3f;

        targetSize = 1.5f;
        targetDuration = 2f;

        StartCoroutine("SpawnMoles");
        SpawnImmediate();
        timer.NewGame();
    }

    private void GameOver()
    {
        StopCoroutine("SpawnMoles");
        ui.GameOver();
        score.GameOver(points);
    }

    /// <summary>
    /// Coroutine to spawn a new mole every time interval.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnMoles()
    {
        while (true)
        {
            if (currentMolesOnScreen < moles.Length && disabledMoles.Count > 0)
            {
                disabledMoles[0].Respawn(location.FindLocation(disabledMoles[0]), RandomMole());
                disabledMoles.RemoveAt(0);
                currentMolesOnScreen++;
            }

            yield return wait;
        }
    }

    /// <summary>
    /// Spawn moles immediatly to make sure at least 5 moles are on screen.
    /// </summary>
    private void SpawnImmediate()
    {
        while (currentMolesOnScreen < 1 && disabledMoles.Count > 0)
        {
            disabledMoles[0].Respawn(location.FindLocation(disabledMoles[0]), RandomMole());
            disabledMoles.RemoveAt(0);
            currentMolesOnScreen++;
        }
    }

    /// <summary>
    /// Picks a random scriptable object for the mole's data.
    /// </summary>
    /// <returns></returns>
    private MoleData RandomMole()
    {
        return new MoleData {size = targetSize, timeOnScreen = targetDuration };
    }

    public void ResetCombo()
    {
        animatorCombo.SetTrigger("breakerTrigger");
        pointsCombo = 0;
        score.UpdateCombo(pointsCombo);
        score.UpdateAccurcy(false);
    }
}