using UnityEngine;

public class Crosshair : MonoBehaviour
{

    [Header("> Cursor Settings")]
    public Texture2D cursorTexture;
    public int cursorSize = 64;

    [HideInInspector]
    public Vector2 position;
    private Vector2 prevMousePosition;
    private Vector2 prevRecoil;

    // Use this for initialization
    void Start()
    {
        UnityEngine.Cursor.visible = false;
        position = Vector2.zero;
        prevMousePosition = Vector2.zero;
    }

    void OnGUI()
    {
        
        Vector2 diff = Event.current.mousePosition - prevMousePosition;
        position = new Vector2(position.x + diff.x, position.y + diff.y);
        prevMousePosition = Event.current.mousePosition;

        if (!this.enabled)
            return;

        GUI.DrawTexture(new Rect(position.x - cursorSize / 2, position.y - cursorSize / 2, cursorSize, cursorSize), cursorTexture);
    }

    public void applyRecoil(float strength)
    {
        Vector2 newRecoil = new Vector2(Random.Range(-strength, strength), Random.Range(-strength, strength));

        position -= prevRecoil;
        position += newRecoil;

        prevRecoil = newRecoil;
    }

}
