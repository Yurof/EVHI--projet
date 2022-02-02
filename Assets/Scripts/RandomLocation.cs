using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using System;

public class RandomLocation : MonoBehaviour
{
    public RectTransform screen;

    private Dictionary<Target, Vector2> reservedLocation = new Dictionary<Target, Vector2>();

    private const float TargetRadius = 50.0f;

    private Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    public float horizontalRandomisedValue = Screen.width / 20;
    public float verticalRandomisedValue = Screen.height / 10;

    private void Start()
    {
    }

    //Trouver la position de la prochaine cible grace au regard et à la distance de fitt's
    public Vector2 FindPosition(float fittDist)
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
}