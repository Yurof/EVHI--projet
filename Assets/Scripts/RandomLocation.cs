using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using System;

public class RandomLocation : MonoBehaviour
{
    public RectTransform screen;

    private Dictionary<Mole, Vector2> reservedLocation = new Dictionary<Mole, Vector2>();

    /// half the width of the screen.
    private float halfWidth;

    /// half the height of the screen.
    private float halfHeight;

    private const float MoleRadius = 50.0f;

    private Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    public float horizontalRandomisedValue = Screen.width / 20;
    public float verticalRandomisedValue = Screen.height / 10;

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
        reservedLocation.Add(m, random);
        return random;
    }

    private Vector2 GetRandom(float fittDist)
    {
        Vector2 resPos;
        Vector2 vect;
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid)
        {
            Vector2 gazePosition = gazePoint.Screen;
            if (gazePosition.x < Screen.width / 2)
            {
                if (gazePosition.y < Screen.height / 2)
                {
                    //haut a droite
                    vect = new Vector2(UnityEngine.Random.Range(0, Screen.width / 2), UnityEngine.Random.Range(0, Screen.height / 2));
                    vect = fittDist * vect.normalized;
                    resPos = screenCenter + vect;
                    if (resPos.x >= Screen.width * 0.95f || resPos.y >= Screen.height * 0.95f || (resPos.x <= Screen.width * 0.05f || resPos.y <= Screen.height * 0.05f))
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width * 0.95f), UnityEngine.Random.Range(Screen.height / 2, Screen.height * 0.95f));
                    }
                }
                else
                {
                    //bas a droite
                    vect = new Vector2(UnityEngine.Random.Range(0, Screen.width / 2), -UnityEngine.Random.Range(0, Screen.height / 2));
                    vect = fittDist * vect.normalized;
                    resPos = screenCenter + vect;
                    if (resPos.x >= Screen.width * 0.95f || resPos.y >= Screen.height * 0.95f || (resPos.x <= Screen.width * 0.05f || resPos.y <= Screen.height * 0.05f))
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width * 0.95f), UnityEngine.Random.Range(Screen.height * 0.05f, Screen.height / 2));
                    }
                }
            }
            else
            {
                if (gazePosition.y < Screen.height / 2)
                {
                    //haut a gauche
                    vect = new Vector2(-UnityEngine.Random.Range(0, Screen.width / 2), UnityEngine.Random.Range(0, Screen.height / 2));

                    vect = fittDist * vect.normalized;
                    resPos = screenCenter + vect;

                    if (resPos.x >= Screen.width * 0.95f || resPos.y >= Screen.height * 0.95f || (resPos.x <= Screen.width * 0.05f || resPos.y <= Screen.height * 0.05f))
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width * 0.05f, Screen.width / 2), UnityEngine.Random.Range(Screen.height / 2, Screen.height * 0.95f));
                    }
                }
                else
                {
                    //bas a gauche
                    vect = new Vector2(-UnityEngine.Random.Range(0, Screen.width / 2), -UnityEngine.Random.Range(0, Screen.height / 2));
                    vect = fittDist * vect.normalized;
                    resPos = screenCenter + vect;
                    if (resPos.x >= Screen.width * 0.95f || resPos.y >= Screen.height * 0.95f || (resPos.x <= Screen.width * 0.05f || resPos.y <= Screen.height * 0.05f))
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width * 0.05f, Screen.width / 2), UnityEngine.Random.Range(Screen.height * 0.05f, Screen.height / 2));
                    }
                }
            }

            return resPos;
        }

        return new Vector2
        {
            x = UnityEngine.Random.Range(0, Screen.width),
            y = UnityEngine.Random.Range(0, Screen.height)
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