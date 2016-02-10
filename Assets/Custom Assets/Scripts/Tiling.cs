using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour
{
    private Camera cam;
    private float spriteWidth;
    private Transform original, clone;
    public bool tileable = true;
    private int tileableFactor = 1;

    void Awake()
    {
        cam = Camera.main;
        original = transform;
        tileableFactor = getBoolFactor(tileable);
    }

    // Use this for initialization
    void Start()
    {
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float camExtend = cam.orthographicSize * Screen.width / Screen.height;
        var rightX = (original.position.x + spriteWidth / 2) - camExtend;
        var leftX = (original.position.x - spriteWidth / 2) + camExtend;
        if (cam.transform.position.x > rightX || cam.transform.position.x < leftX)
        {
            Vector3 pos = original.position;
            pos.x += spriteWidth * getBoolFactor(cam.transform.position.x > rightX);
            moveClone(pos);
            swapInstances();
        }
    }

    private void swapInstances()
    {
        Transform tmp = clone;
        clone = original;
        original = tmp;
    }

    private void moveClone(Vector3 pos)
    {
        if (clone == null)
        {
            clone = (Transform)Instantiate(original, pos, original.rotation);
            clone.parent = original.parent;
            clone.GetComponent<Tiling>().enabled = false;
            Vector3 scale = original.localScale;
            clone.localScale = new Vector3(scale.x * tileableFactor, scale.y, scale.z);
        }
        else
            clone.position = pos;

    }

    private int getBoolFactor(bool condition)
    {
        return ((condition) ? 1 : -1);
    }
}
