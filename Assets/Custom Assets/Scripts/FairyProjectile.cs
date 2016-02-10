using UnityEngine;
using System.Collections;
using System;

public class FairyProjectile : MonoBehaviour
{
    public static Transform target;
    public static LayerMask visibleLayers;
    public float speed = 5f;
    private float step;

    public float moveSpeed = 10f;
    public int maxMoveSteps = 5;
    private int numberMoveSteps = 3;

    Vector2 dir;
    private int strength;

    void Start()
    {
        dir = target.position - transform.position;
        Destroy(this.gameObject, 1f);
    }

    void Update()
    {
        for (int i = 0; i < numberMoveSteps; i++)
        {
            if (checkCollision())
                return;
            transform.Translate(dir * Time.deltaTime * (moveSpeed / maxMoveSteps));
        }
    }


    private bool checkCollision()
    {
        RaycastHit2D hitpoint = Physics2D.CircleCast(transform.position, 0.025f, dir, 0.05f, visibleLayers);
        if (hitpoint.collider != null)
        {
            Player player = hitpoint.collider.GetComponent<Player>();
            if (player != null)
                player.damage(strength);
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    internal void setStrength(int strength)
    {
        this.strength = strength;
    }
}
