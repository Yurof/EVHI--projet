using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
public class GameData
{
    public int[] scores;
    public float[] accuracies;
    public float[] meantimekills;
}

public class GameSave
{
    public void Save(int[] scores, float[] accuracies, float[] meantimekills)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".dat");
        //FileStream filePath = File.Create(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".csv");

        GameData playerScore = new GameData();
        playerScore.scores = scores;
        playerScore.accuracies = accuracies;
        playerScore.meantimekills = meantimekills;

        bf.Serialize(file, playerScore);
        file.Close();

        /*        using (var writer = new StreamWriter(filePath))
                {
                    writer.Write("aaa");
                }*/
    }

    public GameData Load()
    {
        Debug.Log("Name Player" + PlayerPrefs.GetString("name"));
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