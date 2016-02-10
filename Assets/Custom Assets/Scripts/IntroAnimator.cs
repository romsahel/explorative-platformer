using UnityEngine;
using System.Collections;
using System;

public class IntroAnimator : MonoBehaviour
{
    public static GameObject IntroGameObject { get; private set; }
    public static Transform PrincessGameObject { get; private set; }

    public void MoveObject(Transform toMove, Vector3 dest)
    {
        StartCoroutine(MoveObject(toMove, toMove.transform.position, dest));
    }

    void Awake()
    {
        IntroGameObject = GameObject.Find("_Intro");
        PrincessGameObject = IntroGameObject.transform.FindChild("princess");
    }

    //void Start()
    //{
    //    var _intro = GameObject.Find("_Intro");
    //    IntroAnimator.initIntro();
    //    _intro.SetActive(true);
    //}

    IEnumerator MoveObject(Transform toMove, Vector3 startPos, Vector3 dest)
    {
        float progress = 0.0f;
        const float speed = 0.5f;

        while (progress < 1.0f)
        {
            toMove.position = Vector3.Lerp(startPos, dest, Mathf.SmoothStep(0, 1, progress));

            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime * speed;
        }

        toMove.position = dest;
    }

    public static void initIntro()
    {
        Transform parent = initPlayer();
        GameObject.FindObjectOfType<UnityStandardAssets._2D.Camera2DFollow>().enabled = false;
        var player = parent.FindChild("_Player");
        player.gameObject.SetActive(false);
    }

    public static Transform initPlayer()
    {
		var parent = GameObject.Find("__StartPosition").transform;
		var player = parent.FindChild("_Player");
		player.transform.position =  new Vector3(8.12f, 5.3f, -0.4628906f);
        Camera.main.transform.position = new Vector3(8.062345f, 5.263102f, -9.537115f);
        return parent;
    }

    public void endIntro()
    {
        this.GetComponent<UnityStandardAssets._2D.Camera2DFollow>().enabled = true;
        var parent = GameObject.Find("__StartPosition").transform;
        var intro = GameObject.Find("_Intro");
        var princess = intro.transform.FindChild("princess");
        var player = parent.FindChild("_Player");

		player.transform.position =  new Vector3(6.28f, 5.3f, -0.4628906f);
        player.gameObject.SetActive(true);
        princess.gameObject.SetActive(false);
        var door = intro.transform.FindChild("bathroom-door").GetComponent<Door>();
        Door.changeState(door, Door.Type.GOLD, true);

        GameObject.FindObjectOfType<LightManager>().enabled = true;
    }

    public void startIntro()
    {
        MoveObject(this.transform, this.transform.position + Vector3.left * 1.3f);
        this.GetComponent<UnityStandardAssets._2D.Camera2DFollow>().enabled = false;
    }
}
