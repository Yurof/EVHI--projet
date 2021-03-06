using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DIG.GBLXAPI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text comboText;
    public Text accuracyText;
    public Text finalScoreText;
    public Text[] topScoresText;

    public GameLogic GameLogic;
    public GameUI ui;

    private GameSave gameSave = new GameSave();
    private List<int> ListScore = new List<int>();
    private List<float> ListAccuracy = new List<float>();
    private List<float> Listmeantimekill = new List<float>();

    private int missedTarget = 0;
    private int touchedTarget = 0;
    private float accuracy = 0;
    public float scoremeanTimeKill = 0;

    private float accurateThreshold = 80f;
    private float timeThreshold = 1.2f;

    private void Awake()
    {
        //charge les données des utilisateurs
        GameData gd = gameSave.Load();
        if (gd != null)
        {
            ListScore.AddRange(gd.scores);
            ListAccuracy.AddRange(gd.accuracies);
            Listmeantimekill.AddRange(gd.meantimekills);

            if (Queryable.Average(ListAccuracy.AsQueryable()) < accurateThreshold)
            {
                GameLogic.targetSize = 1.5f;
            }
            else
            {
                GameLogic.targetSize = 0.7f;
            }

            if (Queryable.Average(Listmeantimekill.AsQueryable()) < timeThreshold)
            {
                GameLogic.targetDuration = 1.2f;
            }
            else
            {
                GameLogic.targetDuration = 2;
            }
        }
    }

    //affiche les scores
    private void Start()
    {
        ShowTopScores();
    }

    public void UpdateScore(int s)
    {
        scoreText.text = s.ToString("0000");
    }

    public void UpdateCombo(int s)
    {
        comboText.text = s.ToString() + "x";
    }

    public void UpdateAccurcy(bool bol)
    {
        if (bol)
        {
            touchedTarget += 1;
        }
        else
        {
            missedTarget += 1;
        }
        accuracy = 100.0f * touchedTarget / (touchedTarget + missedTarget);

        accuracyText.text = (accuracy).ToString("F2") + "%";
    }

    public void NewGame()
    {
        scoreText.text = "0000";
    }

    public void GameOver(int s)
    {
        finalScoreText.text = s.ToString();
        touchedTarget = 0;
        missedTarget = 0;
        accuracyText.text = "---";
        ListAccuracy.Add(accuracy);
        Listmeantimekill.Add(scoremeanTimeKill);
        SaveData(s);

        GBLXAPI.Init(new GBLConfig());
        GBLXAPI.debugMode = true;
        GBLXAPI.Timers.ResetSlot(0);

        GBL_Interface.userUUID = PlayerPrefs.GetString("name");
        GBL_Interface.SendMeanTimeKill(PlayerPrefs.GetString("name"), scoremeanTimeKill);
        GBL_Interface.SendAccuracy(PlayerPrefs.GetString("name"), accuracy);
        GBL_Interface.SendScore(PlayerPrefs.GetString("name"), s);
    }

    private void SaveData(int s)
    {
        ListScore.Add(s);
        gameSave.Save(ListScore.ToArray(), ListAccuracy.ToArray(), Listmeantimekill.ToArray());
        ShowTopScores();
    }

    //Affichage des scores mise a jour
    private void ShowTopScores()
    {
        topScoresText[0].text = Mathf.Max(ListScore.ToArray()).ToString();

        if (ListAccuracy.Count == 0)
        {
            topScoresText[1].text = "---";
        }
        else
        {
            topScoresText[1].text = (Queryable.Average(ListAccuracy.AsQueryable())).ToString("F2") + "%";
        }

        if (Listmeantimekill.Count == 0)
        {
            topScoresText[1].text = "---";
        }
        else
        {
            topScoresText[2].text = (Queryable.Average(Listmeantimekill.AsQueryable())).ToString("F2") + "s";
        }
    }
}