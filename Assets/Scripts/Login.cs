using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

//
public class Login : MonoBehaviour
{
    public GameObject playerName;
    public GameObject alertName;

    private void Start()
    {
    }

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

        if (!string.IsNullOrEmpty(text) && text != "nul") //verifie que le pseudo est correct
        {
            PlayerPrefs.SetString("name", text);

            if (File.Exists(Application.persistentDataPath + "/" + text + ".dat"))
            {
                SceneManager.LoadScene(2);//profil présent, enclenche le jeu
            }
            else
            {
                SceneManager.LoadScene(1); //pas de profil, enclenche le tutoriel
            }
        }
        else
        {
            alertName.SetActive(true);
        }
    }
}