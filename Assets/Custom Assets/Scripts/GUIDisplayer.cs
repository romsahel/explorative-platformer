using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIDisplayer : MonoBehaviour
{
    private float deltaTime = 0.0f;
    [Header("> Parameters")]
    public bool showFPS = false;
    public Font font;

    [Header("> Link to entities")]
    public Weapon _weapon;
    public Transform player;
    public Text redKeyCounter;
    public Text goldKeyCounter;
    public Text exitKeyCounter;

    [Header("> Weapons")]
    public Image pistolIndicator;
    public Image shotgunIndicator;
    public Image rocketLauncherIndicator;

    [Header("> Health")]
    public Image[] hearts;

    [Header("> Font size")]
    public float smallFontSizeFactor = 1;
    public float normalFontSizeFactor = 1;
    public float bigFontSizeFactor = 1;

    private Player _player;
    private Collector _collector;
    private Text _killScore;
    private Text _fpsCounter;
    private Text _health;
    
    public static Animator _hurtMask = null;
    public static int killCounter;



    void Awake()
    {
        GetComponent<Canvas>().enabled = true;
    }

    void Start()
    {
        _player = player.GetComponent<Player>();
        _collector = player.GetComponent<Collector>();

        _killScore = transform.FindChild("KillScore").GetComponent<Text>();
        _fpsCounter = transform.FindChild("FpsCounter").GetComponent<Text>();
        _health = transform.FindChild("Health").GetComponent<Text>();
        _hurtMask = transform.FindChild("HurtMask").GetComponent<Animator>();

        pistolIndicator.enabled = false;
        shotgunIndicator.enabled = false;
        rocketLauncherIndicator.enabled = false;
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        //float smallFontSize = h * 2 / 105 * smallFontSizeFactor;
        float normalFontSize = h * 2 / 75 * smallFontSizeFactor;
        //float bigFontSize = h * 2 / 50 * smallFontSizeFactor;

        if (showFPS)
            setFpsText();

        _killScore.text = string.Format("Kill Score: {0}", killCounter);
        _health.text = string.Format("Health: {0}", _player.health);
        int currentHealth = _player.health;
        Color newColor = Color.white;
        for (int i = hearts.Length - 1; i >= 0; i--)
        {
            newColor.a = Mathf.Min(Mathf.Max(currentHealth / 10f, 0f), 1f);
            hearts[i].color = newColor;
            currentHealth -= 10;
        }


        goldKeyCounter.text = _collector.getKeyInventory(0).ToString();
        redKeyCounter.text = _collector.getKeyInventory(1).ToString();
        exitKeyCounter.text = _collector.getKeyInventory(2).ToString();

        if (_collector.Tip != null)
            drawTip(w, h, normalFontSize);
    }

    private void setFpsText()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        _fpsCounter.text = text;
    }

    private void drawWeaponStats(int w, int h, float fontSize)
    {
        Rect rect = new Rect(w - 350, h / 2, w, h);
        GUIStyle style = getBasicStyle(fontSize);
        style.alignment = TextAnchor.UpperLeft;
        var str = _weapon.getCurrentWeapon().getString();
        str = str.Replace(" ; ", "\n");
        GUI.Label(rect, str, style);
    }

    private void drawTip(int w, int h, float fontSize)
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(_player.transform.position);
        Rect rect = new Rect(pos.x - w / 2, pos.y - 75, w, h * 2 / 100);
        GUIStyle style = getBasicStyle(fontSize);

        GUI.Label(rect, "Press E to collect " + _collector.Tip, style);
    }

    private GUIStyle getBasicStyle(float fontSize)
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = (int)fontSize;
        style.font = font;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        return style;
    }
}