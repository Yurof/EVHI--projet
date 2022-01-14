using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text comboText;
    public Text accuracyText;
    public Text finalScoreText;
    public Text[] topScoresText;

    private string zero = "0";
    private GameSave gameSave = new GameSave();
    private List<int> topScores = new List<int>();

    private int missedTarget = 0;
    private int touchedTarget = 0;
    private float accuracy = 0;

    private void Awake()
    {
        topScores.AddRange(gameSave.Load());
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

        CheckNewHighScore(s);
    }

    private void CheckNewHighScore(int s)
    {
        if (s < topScores[0] || topScores.Contains(s))
            return;

        topScores[4] = s;
        topScores.Sort();
        topScores.Reverse();

        gameSave.Save(topScores.ToArray());

        ShowTopScores();
    }

    private void ShowTopScores()
    {
        for (int i = 0; i < 1; i++)
        {
            topScoresText[i].text = topScores[i].ToString();
        }
    }
}