using UnityEngine;

//Change le curseur de souris
public class setCursor : MonoBehaviour
{
    public Texture2D crosshair;

    private void Start()
    {
        Vector2 cursorOffset = new Vector2(crosshair.width / 2, crosshair.height / 2);
        Cursor.SetCursor(crosshair, cursorOffset, CursorMode.Auto);
    }
}