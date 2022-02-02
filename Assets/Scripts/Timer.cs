using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//Gestion du timer
public class Timer : MonoBehaviour
{
    public Text timerText;

    public int startTime;

    private WaitForSeconds wait = new WaitForSeconds(1);
    private int time;

    public UnityAction OnTimeOut;

    public void NewGame()
    {
        time = startTime;
        timerText.text = startTime.ToString();
        StartCoroutine("StartTimer");
    }

    private IEnumerator StartTimer()
    {
        while (true)
        {
            yield return wait;
            time--;
            timerText.text = time.ToString();
            if (time == 0)
                break;
        }
        OnTimeOut();
    }
}