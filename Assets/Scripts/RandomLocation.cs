using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class RandomLocation : MonoBehaviour
{
    public RectTransform screen;

    private Dictionary<Mole, Vector2> reservedLocation = new Dictionary<Mole, Vector2>();

    /// half the width of the screen.
    private float halfWidth;

    /// half the height of the screen.
    private float halfHeight;

    private const float MoleRadius = 50.0f;

    private void Start()
    {
        halfWidth = screen.sizeDelta.x;
        halfHeight = screen.sizeDelta.y;
    }

    public void NewGame()
    {
        reservedLocation.Clear();
    }

    public Vector2 FindLocation(Mole m, float fittDist)
    {
        Vector2 random = GetRandom(fittDist);

        // number of attempt to find a non-overlaping location
        int numOfAttempts = 0;

        // if number of attempts reaches 25, just return an overlaping location this time.
        while (CheckOverlap(m, random) == false && numOfAttempts < 25)
        {
            random = GetRandom(fittDist);
            numOfAttempts++;
        }

        //Debug.Log(numOfAttempts);
        reservedLocation.Add(m, random);
        return random;
    }

    private Vector2 GetRandom(float fittDist)
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid)
        {
            Vector2 gazePosition = gazePoint.Screen;
            /*            Debug.Log(Screen.width);
                        Debug.Log(Mathf.RoundToInt(gazePosition.x));*/
            if (gazePosition.x < Screen.width / 2)
            {
                if(gazePosition.y < Screen.height / 2)
                {
                    return new Vector2(Random.Range(Screen.width / 2, Screen.width), Random.Range(Screen.height / 2, Screen.height));
                }
                else
                {
                    return new Vector2(Random.Range(Screen.width / 2, Screen.width), Random.Range(0, Screen.height / 2));
                }
                
            }
            else
            {
                if (gazePosition.y < Screen.height / 2)
                {
                    return new Vector2(Random.Range(0, Screen.width / 2), Random.Range(Screen.height / 2, Screen.height));
                }
                else
                {
                    return new Vector2(Random.Range(0, Screen.width / 2), Random.Range(0, Screen.height / 2));
                }
            }
        }
        /*        Debug.Log("x 3: " + Random.Range(0, halfWidth));
                Debug.Log("y 3: " + Random.Range(0, halfHeight));*/
        return new Vector2
        {
            x = Random.Range(0, Screen.width),
            y = Random.Range(0, Screen.height)
        };
    }

    public void FreeLocation(Mole m)
    {
        reservedLocation.Remove(m);
    }

    private bool CheckOverlap(Mole m, Vector2 newPos)
    {
        foreach (KeyValuePair<Mole, Vector2> entry in reservedLocation)
        {
            float minDistance = (MoleRadius * m.data.size) + (MoleRadius * entry.Key.data.size);

            float distance = Vector2.Distance(newPos, entry.Value);

            if (distance < minDistance)
                return false;
        }

        return true;
    }
}