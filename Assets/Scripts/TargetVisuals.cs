using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetVisuals : MonoBehaviour
{
    public RectTransform scale;

    public RectTransform movingImage;

    public void Respawn(TargetData data)
    {
        Color initialColor = movingImage.GetComponent<Image>().color;
        movingImage.GetComponent<Image>().color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

        scale.localScale = new Vector2(data.size, data.size);

        StartCoroutine("Animate", data);
    }

    private IEnumerator Animate(TargetData data)
    {
        Color initialColor = movingImage.GetComponent<Image>().color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < data.timeOnScreen)
        {
            elapsedTime += Time.deltaTime;
            movingImage.GetComponent<Image>().color = Color.Lerp(initialColor, targetColor, elapsedTime / data.timeOnScreen);
            yield return null;
            /*yield return new WaitForEndOfFrame();*/
        }
    }
}