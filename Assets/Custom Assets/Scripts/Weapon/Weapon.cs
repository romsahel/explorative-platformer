using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;
using Assets.Custom_Assets.Scripts.Weapon;

public class Weapon : MonoBehaviour
{
    public enum Types
    {
        Pistol = 0,
        Shotgun,
        RocketLauncher,
        END
    }
    public Transform[] weaponsTransform;
    private IWeapon[] weapons = new IWeapon[(int)Types.END];
    private Types selectedWeapon = Types.END;

    public LayerMask hittableLayers;
    private Transform trailPrefab;
    public Transform player;
    public float spawnRate = 10;

    private float timeToFire = 0f;
    private bool rightSide = true;

    private float jammed = 0f;

    private AudioClip fireringWeapon;
    AudioSource audioSource;

    //private int shotToEvolve = 5;
    private const double degreeToRadian = 180 / Math.PI;

    public float Angle { get; private set; }

    // Use this for initialization
    void Start()
    {
        fireringWeapon = Resources.Load("Audio/firering_weapon") as AudioClip;
        audioSource = GetComponent<AudioSource>();
        changeSelectedWeapon(Types.END);
    }

    private void changeSelectedWeapon(Types newSelection)
    {
        if (newSelection != Types.END && weapons[(int)newSelection] == null)
            return;
        selectedWeapon = newSelection;

        for (int i = 0; i < (int)Types.END; i++)
            weaponsTransform[i].gameObject.SetActive((i == (int)selectedWeapon));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
            changeSelectedWeapon(Types.Pistol);
        else if (Input.GetKey(KeyCode.Alpha2))
            changeSelectedWeapon(Types.Shotgun);
        else if (Input.GetKey(KeyCode.Alpha3))
            changeSelectedWeapon(Types.RocketLauncher);
        else if (Input.GetKey(KeyCode.Quote))
            changeSelectedWeapon(Types.END);

        //_crosshair.enabled = (selectedWeapon != Types.END);
        if (selectedWeapon == Types.END)
            return;

        flipWeapon(player.localScale.x > 0);
        //mouse = Camera.main.ScreenToWorldPoint(_crosshair.position);
        //Angle = rotateTransform(getCurrentTransform(), transform.position, mouse);

        if (Input.GetKey(KeyCode.Space))
        {
            if (jammed <= 0f && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / getCurrentWeapon().getCharacteristics()[(int)IWeapon.Stats.FIRERATE];
                Shoot();
            }
        }
    }

    private float rotateTransform(Transform toRotate, Vector3 origin, Vector3 end)
    {
        float angle = (float)(-Math.Atan2(end.y - origin.y, end.x - origin.x) * degreeToRadian);
        if (rightSide)
            toRotate.rotation = Quaternion.Euler(0, 0, angle);
        else
            toRotate.rotation = Quaternion.Euler(0, 180, (-angle - 180));
        return angle;
    }
    private void flipWeapon(bool isScaleNormal)
    {
        //float offset = 0.092f;
        if (!isScaleNormal && rightSide)
        {
            //for (int i = 0; i < weapons.Length; i++)
            //{
            //    Vector3 newPosition = weaponsTransform[i].position;
            //    newPosition.x -= offset;
            //    weaponsTransform[i].position = newPosition;
            //}
            //getCurrentTransform().rotation = Quaternion.Euler(0, 180, 0);
            rightSide = false;
            Angle = 180;
        }
        else if (isScaleNormal && !rightSide)
        {
            //for (int i = 0; i < weapons.Length; i++)
            //{
            //    Vector3 newPosition = weaponsTransform[i].position;
            //    newPosition.x += offset;
            //    weaponsTransform[i].position = newPosition;
            //}
            //getCurrentTransform().rotation = Quaternion.Euler(0, 0, 0);
            Angle = 0;
            rightSide = true;
        }
    }

    private void Shoot()
    {
        var weapon = getCurrentWeapon();
        float spread = weapon.getCharacteristic(IWeapon.Stats.NUMBEROFPROJECTILES);
        Transform[] bullets = new Transform[(int)weapon.getCharacteristic(IWeapon.Stats.NUMBEROFPROJECTILES)];
        Vector2[] destinations = new Vector2[bullets.Length];
        Transform trail = getCurrentTransform().FindChild("trail");

        if (bullets.Length == 1)
            createBullet(bullets, destinations, trail, 0, 0);
        else
            for (int i = 0; i < bullets.Length; i++)
                createBullet(bullets, destinations, trail, i, spread);

        audioSource.PlayOneShot(fireringWeapon, 0.7F);

        player.GetComponentInParent<PlatformerMotor2D>().pushBack(rightSide, weapon.getCharacteristic(IWeapon.Stats.PUSHBACK));
        var chances = UnityEngine.Random.Range(0, 100) * weapon.getCharacteristic(IWeapon.Stats.JAM);
        if (chances > 95)
        {
            Debug.Log(chances + ": JAMMED");
        }
    }

    private void createBullet(Transform[] bullets, Vector2[] destinations, Transform trail, int i, float spread)
    {
        getCurrentWeapon().BehaviorTracker.signalShooting();
        float radius = 1;
        float newAngle = Angle + UnityEngine.Random.Range(-spread / 2, spread / 2);
        Quaternion q = Quaternion.AngleAxis(newAngle, Vector3.back);
        Vector3 newPosition = transform.position + q * Vector3.right * radius;
        destinations[i] = new Vector2(newPosition.x, newPosition.y);
        bullets[i] = (Transform)Instantiate(trail, trail.position, trail.rotation);
        rotateTransform(bullets[i], transform.position, destinations[i]);
        bullets[i].gameObject.SetActive(true);
        Vector2 pos = transform.position;
        Vector2 dir = (destinations[i] - pos);
        bullets[i].GetComponent<Bullet>().setHitParameters(transform.position, dir, hittableLayers, getCurrentWeapon(), weapons);
    }

    public Transform getCurrentTransform()
    {
        return weaponsTransform[(int)selectedWeapon];
    }

    public IWeapon getCurrentWeapon()
    {
        if (selectedWeapon == Types.END)
            return null;
        return weapons[(int)selectedWeapon];
    }

    public void addWeapon(Types type, int efficiencyPercentage)
    {
        switch (type)
        {
            case Types.Pistol:
                weapons[(int)type] = new Pistol(efficiencyPercentage);
                break;
            case Types.Shotgun:
                weapons[(int)type] = new Shotgun(efficiencyPercentage);
                break;
            case Types.RocketLauncher:
                weapons[(int)type] = new RocketLauncher(efficiencyPercentage);
                break;
        }
    }
    public void addWeapon(Types type, float[] characteristics)
    {
        switch (type)
        {
            case Types.Pistol:
                weapons[(int)type] = new Pistol(characteristics);
                GameObject.Find("__GUIDisplayer").GetComponent<GUIDisplayer>().pistolIndicator.enabled = true;
                break;
            case Types.Shotgun:
                weapons[(int)type] = new Shotgun(characteristics);
                GameObject.Find("__GUIDisplayer").GetComponent<GUIDisplayer>().shotgunIndicator.enabled = true;
                break;
            case Types.RocketLauncher:
                weapons[(int)type] = new RocketLauncher(characteristics);
                GameObject.Find("__GUIDisplayer").GetComponent<GUIDisplayer>().rocketLauncherIndicator.enabled = true;
                break;
        }
        changeSelectedWeapon(type);
    }
}
