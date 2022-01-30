using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text comboText;
    public Text accuracyText;
    public Text finalScoreText;
    public Text[] topScoresText;

    private string zero = "0";
    private GameSave gameSave = new GameSave();
    private List<int> ListScore = new List<int>();
    private List<float> ListAccuracy = new List<float>();

    private int missedTarget = 0;
    private int touchedTarget = 0;
    private float accuracy = 0;

    private void Awake()
    {
        ListScore.AddRange(gameSave.Load());
    }

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
        scoreText.text = zero;
    }

    public void GameOver(int s)
    {
        finalScoreText.text = s.ToString();
        touchedTarget = 0;
        Debug.Log("MISSED TARGET REMIS A 0");
        missedTarget = 0;
        accuracyText.text = zero;
        SaveData(s);
    }

    private void SaveData(int s)
    {
        ListScore.Add(s);
        ListAccuracy.Add(accuracy);

        gameSave.Save(ListScore.ToArray(), ListAccuracy.ToArray());
        ShowTopScores();
    }

    private void ShowTopScores()
    {
        topScoresText[0].text = Mathf.Max(ListScore.ToArray()).ToString();

        float sum = 0;
        for (var i = 0; i < ListAccuracy.Count; i++)
        {
            sum += ListAccuracy[i];
        }

        topScoresText[1].text = (sum / ListAccuracy.Count).ToString("F2") + "%";
    }
}