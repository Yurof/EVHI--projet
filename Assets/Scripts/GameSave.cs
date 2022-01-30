using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
internal class GameData
{
    public int[] scores;
    public float[] accuracies;
}

public class GameSave
{
    public void Save(int[] scores, float[] accuracies)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".dat");
        //FileStream filePath = File.Create(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".csv");
        Debug.Log(Application.persistentDataPath);

        GameData playerScore = new GameData();
        playerScore.scores = scores;
        playerScore.accuracies = accuracies;

        bf.Serialize(file, playerScore);
        file.Close();

        /*        using (var writer = new StreamWriter(filePath))
                {
                    writer.Write("aaa");
                }*/
    }

    public int[] Load()
    {
        if (File.Exists(Application.persistentDataPath + "/" + PlayerPrefs.GetString("name") + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerScore.dat", FileMode.Open);
            GameData playerScore = (GameData)bf.Deserialize(file);
            file.Close();

            return playerScore.scores;
        }

        return new int[5];
    }
}