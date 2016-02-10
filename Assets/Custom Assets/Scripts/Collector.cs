using UnityEngine;
using System.Collections;
using System.Linq;



public class Collector : MonoBehaviour
{
    public LayerMask m_whatIsCollectible;
    public LayerMask m_whatIsDoor;
    public LayerMask m_whatIsTrap;


    public float trapFrequency = 1f;
    public float minDistanceFromEnv = 0.01f;

    private float trapTrigger;

    private int[] keyInventory;
    private Collider2D _collider2D { get; set; }
    private Player _player;
    private AudioClip drinking;

    private AudioClip keyPickup;
    private AudioClip unlock_door;
    AudioSource audioSource;

    private bool[] alreadyPickedUp = { false, false, false };
    public Door previousDoor = null;
    public Transform previousRoom, currentRoom;
    public Vector3 doorCollision;

    public string Tip { get; set; }

    void Awake()
    {
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("OpenedDoors"));

        keyPickup = Resources.Load("Audio/keyPickup") as AudioClip;
        unlock_door = Resources.Load("Audio/unlock_door") as AudioClip;
        drinking = Resources.Load("Audio/drinking") as AudioClip;
        audioSource = GetComponent<AudioSource>();

        _collider2D = GetComponent<Collider2D>();
        _player = GetComponent<Player>();
        keyInventory = new int[3];

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkDoors();
        checkTraps();
    }

    void Update()
    {
        Tip = null;
        if (Input.GetKey(KeyCode.E))
            checkCollectibles();
        changeTip();
    }

    private void changeTip()
    {
        Bounds checkBounds = _collider2D.bounds;
        checkBounds.extents += Vector3.one * minDistanceFromEnv;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(checkBounds.min, checkBounds.max, m_whatIsCollectible);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collectible collectible = colliders[i].gameObject.GetComponent<Collectible>();
            switch (collectible.getType())
            {
                case Collectible.Type.Health:
                    if (alreadyPickedUp[0])
                        return;
                    break;
                case Collectible.Type.Key:
                    if (alreadyPickedUp[1])
                        return;
                    break;
                case Collectible.Type.Weapon:
                    Weapon.Types w = collectible.weaponType;
                    Tip = System.Text.RegularExpressions.Regex.Replace(w.ToString(), "(\\B[A-Z])", " $1")
                        + ".\nPress " + ((int)w + 1) + " to select.";
                    return;
            }
            Tip = collectible.getType().ToString();
        }
    }

    private void checkCollectibles()
    {
        Bounds checkBounds = _collider2D.bounds;
        checkBounds.extents += Vector3.one * minDistanceFromEnv;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(checkBounds.min, checkBounds.max, m_whatIsCollectible);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collectible collectible = colliders[i].gameObject.GetComponent<Collectible>();
            switch (collectible.getType())
            {
                case Collectible.Type.Health:
                    //audioSource = GetComponent<AudioSource>();
                    _player.setCurrentState(Player.State.DRINKING);
                    audioSource.PlayOneShot(drinking, 1.0F);
                    _player.health += 10;
                    alreadyPickedUp[0] = true;
                    break;
                case Collectible.Type.Key:
                    keyInventory[(int)collectible.getKeyType()]++;
                    /*if (collectible.getKeyType() == Door.Type.GOLD)
                        audioSource.PlayOneShot(key1, 0.5F);
                    else
                        audioSource.PlayOneShot(key2, 0.5F);*/

                    audioSource.PlayOneShot(keyPickup, 0.2F);
                    _player.setCurrentState(Player.State.PICKING);
                    alreadyPickedUp[1] = true;
                    break;
                case Collectible.Type.Weapon:
                    //_player.GetComponentInChildren<Weapon>().addWeapon(collectible.weaponType, collectible.efficiencyPercentage);
                    _player.GetComponentInChildren<Weapon>().addWeapon(collectible.weaponType, collectible.GetComponent<CollectibleWeapon>().getArray());
                    alreadyPickedUp[2] = true;
                    break;
                default:
                    break;
            }
            Destroy(colliders[i].gameObject);
        }
    }

    private void checkDoors()
    {
        Bounds checkBounds = _collider2D.bounds;
        checkBounds.extents += Vector3.one * minDistanceFromEnv * 3;
        Collider2D overlap = Physics2D.OverlapArea(checkBounds.min, checkBounds.max, m_whatIsDoor);
        if (overlap != null)
        {
            Door door = overlap.gameObject.GetComponent<Door>();
            if (previousDoor == door)
                return;

            if (!door.isOpen())
            {
                if (door.isLocked)
                {
                    if (keyInventory[(int)door.getDoorType()] == 0)
                        return;

                    audioSource.PlayOneShot(unlock_door, 0.1F);
                    keyInventory[(int)door.getDoorType()]--;
                }
            }

            var newRoom = door.open();

            previousRoom = currentRoom;
            if (newRoom == currentRoom)
                currentRoom = null;
            else
                currentRoom = newRoom;
            previousDoor = door;
            doorCollision = transform.position;
        }
        else if (previousDoor != null)
        {
            // came back to the same side of the door
            if ((doorCollision.x < previousDoor.transform.position.x) == (transform.position.x < previousDoor.transform.position.x))
                currentRoom = previousRoom;
            previousDoor = null;
        }
    }
    private void checkTraps()
    {
        if (trapTrigger < trapFrequency)
        {
            trapTrigger += Time.deltaTime;
            if (trapTrigger < trapFrequency)
                return;
        }

        Bounds checkBounds = _collider2D.bounds;
        checkBounds.extents += Vector3.one * minDistanceFromEnv * 2;

        Collider2D[] colliders = Physics2D.OverlapAreaAll(checkBounds.min, checkBounds.max, m_whatIsTrap);
        if (colliders.Length > 0)
        {
            _player.damage(5);
            trapTrigger = 0f;
        }
    }

    public int getKeyInventory(int index)
    {
        return keyInventory[index];
    }
}