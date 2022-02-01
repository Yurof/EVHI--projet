using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;

using DIG.GBLXAPI;
using TinCan;

[Serializable]
public class GameData
{
    public int[] scores;
    public float[] accuracies;
    public float[] meantimekills;
}

public class GameSave
{
    public Text statementText;

    public void Save(int[] scores, float[] accuracies, float[] meantimekills)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".dat");

        GameData playerScore = new GameData();
        playerScore.scores = scores;
        playerScore.accuracies = accuracies;
        playerScore.meantimekills = meantimekills;

        bf.Serialize(file, playerScore);
        file.Close();
    }

    public GameData Load()
    {
        if (File.Exists(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".dat", FileMode.Open);
            GameData playerScore = (GameData)bf.Deserialize(file);
            file.Close();

            return playerScore;
        }
        return null;
    }
}