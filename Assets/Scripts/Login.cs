using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class Login : MonoBehaviour
{
    public GameObject playerName;
    public GameObject alertName;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayGame();
        }
    }

    public void PlayGame()
    {
        string text = playerName.GetComponent<TMP_InputField>().text;

        if (!string.IsNullOrEmpty(text) && text != "nul")
        {
            PlayerPrefs.SetString("name", text);

            if (File.Exists(Application.persistentDataPath + "/" + text + ".dat"))
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            alertName.SetActive(true);
        }
    }
}