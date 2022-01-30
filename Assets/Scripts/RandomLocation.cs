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

    private Vector2 topRightCenter = new Vector2(Screen.width * 0.75f, Screen.height * 0.75f);
    private Vector2 topLeftCenter = new Vector2(Screen.width * 0.25f, Screen.height * 0.75f);
    private Vector2 bottomRightCenter = new Vector2(Screen.width * 0.75f, Screen.height * 0.25f);
    private Vector2 bottomLeftCenter = new Vector2(Screen.width * 0.25f, Screen.height * 0.25f);

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

        //Debug.Log(numOfAttempts);
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
            /*            Debug.Log(Screen.width);
                        Debug.Log(Mathf.RoundToInt(gazePosition.x));*/
            if (gazePosition.x < Screen.width / 2)
            {
                if(gazePosition.y < Screen.height / 2)
                {


                    vect = new Vector2(topRightCenter.x - gazePosition.x , topRightCenter.y - gazePosition.y);
                    vect = fittDist * vect.normalized;
                    resPos = gazePosition + vect;
                    if(resPos.x >= Screen.width || resPos.y >= Screen.height)
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width), UnityEngine.Random.Range(Screen.height / 2, Screen.height));
                    }
                    /*return new Vector2(Random.Range(Screen.width / 2, Screen.width), Random.Range(Screen.height / 2, Screen.height));*/
                }
                else
                {
                    vect = new Vector2(bottomRightCenter.x - gazePosition.x, topRightCenter.y - gazePosition.y);
                    vect = fittDist * vect.normalized;
                    resPos = gazePosition + vect;
                    if (resPos.x >= Screen.width || resPos.y >= Screen.height)
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width), UnityEngine.Random.Range(Screen.height / 2, Screen.height));
                    }
                    /*return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width), UnityEngine.Random.Range(0, Screen.height / 2));*/
                }
                
            }
            else
            {
                if (gazePosition.y < Screen.height / 2)
                {
                    vect = new Vector2(topLeftCenter.x - gazePosition.x, topRightCenter.y - gazePosition.y);
                    vect = fittDist * vect.normalized;
                    resPos = gazePosition + vect;
                    if (resPos.x >= Screen.width || resPos.y >= Screen.height)
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width), UnityEngine.Random.Range(Screen.height / 2, Screen.height));
                    }
                    /*return new Vector2(UnityEngine.Random.Range(0, Screen.width / 2), UnityEngine.Random.Range(Screen.height / 2, Screen.height));*/
                }
                else
                {
                    vect = new Vector2(bottomLeftCenter.x - gazePosition.x, topRightCenter.y - gazePosition.y);
                    vect = fittDist * vect.normalized;
                    resPos = gazePosition + vect;
                    if (resPos.x >= Screen.width || resPos.y >= Screen.height)
                    {
                        return new Vector2(UnityEngine.Random.Range(Screen.width / 2, Screen.width), UnityEngine.Random.Range(Screen.height / 2, Screen.height));
                    }
                    /*return new Vector2(UnityEngine.Random.Range(0, Screen.width / 2), UnityEngine.Random.Range(0, Screen.height / 2));*/
                }
            }
            do
            {
                resPos.x += UnityEngine.Random.Range(-horizontalRandomisedValue, horizontalRandomisedValue);
                resPos.y += UnityEngine.Random.Range(-verticalRandomisedValue, verticalRandomisedValue);
            } while (resPos.x >= Screen.width || resPos.x <= 0 || resPos.y >= Screen.height || resPos.y <= 0);

            Debug.Log("Position du regarde : " + gazePosition);
            Debug.Log("Position de la cible : " + resPos);
            return resPos;
        }
        /*        Debug.Log("x 3: " + Random.Range(0, halfWidth));
                Debug.Log("y 3: " + Random.Range(0, halfHeight));*/
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