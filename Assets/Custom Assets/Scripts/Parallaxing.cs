using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour
{

    public Transform[] backgrounds;
    private float[] parallaxScales;
    public float smoothing = 1f;

    private Transform cam;
    private Vector3 previousPos;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start()
    {
        previousPos = cam.position;

        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
            parallaxScales[i] = backgrounds[i].position.z * -1;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Vector3 pos = backgrounds[i].position;
            pos.x += (previousPos.x - cam.position.x) * parallaxScales[i];
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, pos, smoothing * Time.deltaTime);
        }
        previousPos = cam.position;
    }
}
