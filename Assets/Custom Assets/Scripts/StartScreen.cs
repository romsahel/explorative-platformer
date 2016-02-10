using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour
{
    private Animator mainScreenAnimator;
    private static bool enableIntro = true;
    public static LightManager _lightManager { get; private set; }

    // Use this for initialization
    void Awake()
    {
        setShown(false);
        _lightManager = FindObjectOfType<LightManager>();
        mainScreenAnimator = transform.FindChild("MainScreen").GetComponent<Animator>();
    }

    public void setShown(bool active)
    {
        foreach (Transform obj in this.transform)
        {
            obj.gameObject.SetActive(active);
            Animator animator = obj.GetComponent<Animator>();
            if (animator != null)
                animator.enabled = active;
        }
    }

    void Start()
    {
        setShown(true);
        mainScreenAnimator.enabled = false;
        IntroAnimator.IntroGameObject.GetComponent<Animator>().enabled = false;
        _lightManager.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        mainScreenAnimator.enabled = true;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Tab))
		{
            _lightManager.enabled = true;
            if (enableIntro && !Input.GetKey(KeyCode.Tab))
            {
                IntroAnimator.initIntro();
				GetComponent<AudioSource>().Play();
                GetComponent<Animator>().Play("intro-texts");
                this.enabled = false;
            }
            else
            {
                IntroAnimator.initPlayer();
                setShown(false);
                this.enabled = false;
			}
			enableIntro = false;
        }

        if (Input.GetKey(KeyCode.Return))
        {
            _lightManager.enabled = true;
            setShown(false);
            this.enabled = false;
        }
    }
}
