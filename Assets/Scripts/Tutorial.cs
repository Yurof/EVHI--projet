using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public Text Welcome;

    public GameObject targetcat;
    public AudioSource hitaudio;

    private List<Vector2> listSpawn = new List<Vector2>();
    private int i = 0;
    private float offsetx = 0;
    private float offsety = 0;

    private void Start()
    {
        Welcome.text = "Bienvenue " + PlayerPrefs.GetString("name");

        Invoke("WelcomeFunction", 2);

        listSpawn.Add(new Vector2(7.19f, -3.3f));
        listSpawn.Add(new Vector2(-7.19f, -3.3f));
        listSpawn.Add(new Vector2(0f, 3.68f));
    }

    public void WelcomeFunction()
    {
        Welcome.text = "Clique sur mon ventre si tu veux passer";
        Invoke("NewGame", 1.5f);
    }

    public void NewGame()
    {
        Destroy(Welcome);
        targetcat.SetActive(true);
    }

    public void clicked()
    {
        hitaudio.Play();
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid)
        {
            Vector2 clickPos = (Vector2)Input.mousePosition;
            Vector2 offset = clickPos - gazePoint.Screen;
            offsetx += offset.x;
            offsety += offset.y;
        }

        if (i < listSpawn.Count)
        {
            targetcat.transform.position = listSpawn[i];
            if (i == 1)
            {
                targetcat.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (i == 2)
            {
                targetcat.transform.Rotate(0, 0, 180);
            }
            i++;
        }
        else
        {
            PlayerPrefs.SetFloat("offsetx", offsetx / listSpawn.Count);
            PlayerPrefs.SetFloat("offsety", offsety / listSpawn.Count);
            SceneManager.LoadScene(2);
        }
    }
}