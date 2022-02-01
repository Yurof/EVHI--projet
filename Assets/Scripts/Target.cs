using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Target : MonoBehaviour
{
    /// Used to animate and scale the target
    public TargetVisuals visuals;

    public UnityAction<Target, bool> OnTargetDied;

    [HideInInspector]
    public TargetData data;

    /// Spawn the target at a given position.
    public void Respawn(Vector2 pos, TargetData d)
    {
        data = d;

        data.spawnTime = Time.time;

        gameObject.GetComponent<RectTransform>().position = pos;
        gameObject.SetActive(true);
        StartCoroutine("Timer");

        visuals.Respawn(data);
    }

    /// <summary>
    /// Despawn the target.
    /// </summary>
    public void Despawn()
    {
        if (gameObject.activeSelf == false)
            return;

        gameObject.SetActive(false);
    }

    /// Called when the target is clicked.
    public void TargetClicked()
    {
        StopCoroutine("Timer");
        OnTargetDied(this, true);
        Despawn();
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(data.timeOnScreen);
        OnTargetDied(this, false);
        Despawn();
    }
}