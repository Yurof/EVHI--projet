using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public TargetVisuals visuals;

    public UnityAction<Target, bool> OnTargetDied;

    [HideInInspector]
    public TargetData data;

    // Creer la cible a une certaine position
    public void Respawn(Vector2 pos, TargetData d)
    {
        data = d;

        data.spawnTime = Time.time;

        gameObject.GetComponent<RectTransform>().position = pos;
        gameObject.SetActive(true);
        StartCoroutine("Timer");

        visuals.Respawn(data);
    }

    //desactive l'objet
    public void Despawn()
    {
        if (gameObject.activeSelf == false)
            return;

        gameObject.SetActive(false);
    }

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