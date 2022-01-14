using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject playCanvas;
    public GameObject scoreCanvas;
    public GameObject GazePlot;

    public void NewGame()
    {
        scoreCanvas.SetActive(false);
        playCanvas.SetActive(true);
    }

    public void GameOver()
    {
        playCanvas.SetActive(false);
        scoreCanvas.SetActive(true);
    }

    public void ShowGaze()
    {
        GazePlot.SetActive(!GazePlot.activeInHierarchy);
    }
}