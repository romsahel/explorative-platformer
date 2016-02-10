using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Collectible : MonoBehaviour
{
    public enum Type
    {
        Health,
        Key,
        Weapon,
    }

    //private const float RANGE = 0.1f;
    //private Vector3[] positions;
    //private int target = 0;
    //private bool ready = true;
    
    [SerializeField]
    private Type m_Type = Type.Health;
    [SerializeField]
    private Door.Type m_KeyType;

    public Weapon.Types weaponType;
    public int efficiencyPercentage;
    
    void Awake()
    {
    }

    //void Update()
    //{
    //    //if (ready)
    //    StartCoroutine(MoveObject());
    //}

    //IEnumerator MoveObject()
    //{
    //    float progress = 0.0f;
    //    const float speed = 2f;
    //    Vector3 startPos = transform.position;

    //    ready = false;
    //    while (progress < 1.0f)
    //    {
    //        transform.position = Vector3.Lerp(startPos, positions[target], Mathf.SmoothStep(0, 1, progress));

    //        yield return new WaitForEndOfFrame();
    //        progress += Time.deltaTime * speed;
    //    }
    //    ready = true;

    //    transform.position = positions[target];
    //    target = ++target % positions.Length;
    //}

    public Type getType()
    {
        return m_Type;
    }

    public Door.Type getKeyType()
    {
        return m_KeyType;
    }
}
