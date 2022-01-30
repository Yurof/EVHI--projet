using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Mole : MonoBehaviour
{
    /// Used to animate and scale the mole
    public MoleVisuals visuals;

    public UnityAction<Mole, bool> OnMoleDied;

    [HideInInspector]
    public MoleData data;

    /// Spawn the mole at a given position.
    public void Respawn(Vector2 pos, MoleData d)
    {
        data = d;

        data.spawnTime = Time.time;

        gameObject.GetComponent<RectTransform>().position = pos;
        gameObject.SetActive(true);
        StartCoroutine("Timer");

        visuals.Respawn(data);
    }

    /// <summary>
    /// Despawn the mole.
    /// </summary>
    public void Despawn()
    {
        if (gameObject.activeSelf == false)
            return;

        gameObject.SetActive(false);
    }

    /// Called when the mole is clicked.
    public void MoleClicked()
    {
        StopCoroutine("Timer");
        OnMoleDied(this, true);
        Despawn();
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(data.timeOnScreen);
        OnMoleDied(this, false);
        Despawn();
    }
}