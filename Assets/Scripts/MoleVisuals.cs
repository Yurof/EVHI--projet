using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoleVisuals : MonoBehaviour
{
    public RectTransform scale;
    /*    public Image front;
        public Image back;*/

    public RectTransform movingImage;
    /*    public RectTransform target;
    */
    private float step;

    public void Respawn(MoleData data)
    {
        Color initialColor = movingImage.GetComponent<Image>().color;
        movingImage.GetComponent<Image>().color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

        scale.localScale = new Vector2(data.size, data.size);

        /*        front.color = data.color;
                back.color = data.color;*/

        /*movingImage.localPosition = Vector2.zero;*/
        /*step = (Vector2.Distance(movingImage.localPosition, target.localPosition) / data.timeOnScreen) * Time.deltaTime;*/

        StartCoroutine("Animate", data);
    }

    private IEnumerator Animate(MoleData data)
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