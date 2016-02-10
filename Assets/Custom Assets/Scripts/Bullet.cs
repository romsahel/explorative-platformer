using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 20f;
    public int maxMoveSteps = 5;
    private int numberMoveSteps;

    private Vector2 dir;
    private LayerMask hittableLayers;
    private IWeapon currentWeapon;
    private IWeapon[] weapons;
    private float area;
    private Vector2 playerPosition;

    void Start()
    {
        // move the trail so that it starts at the weapon
        //transform.Translate(Vector3.up * 0.01f);
        Destroy(this.gameObject, (15f / moveSpeed));
        numberMoveSteps = (int)(moveSpeed / maxMoveSteps);
    }

    void Update()
    {
        for (int i = 0; i < numberMoveSteps; i++)
        {
            if (checkCollision())
                return;
            transform.Translate(Vector3.right * Time.deltaTime * (moveSpeed / maxMoveSteps));
        }
    }

    private bool checkCollision()
    {
        Debug.DrawRay(transform.position, dir);
        RaycastHit2D hitpoint = Physics2D.CircleCast(transform.position, 0.025f, dir, 0.05f, hittableLayers);
        if (hitpoint.collider != null)
        {
            if (area > 0.3f)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, area);
                Debug.Log("Rocket launcher hit: " + colliders.Length);
                foreach (Collider2D hit in colliders)
                    damageEnemy(hit);
            }
            else
                damageEnemy(hitpoint.collider);
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    private void damageEnemy(Collider2D hit)
    {
        Enemy enemy = hit.GetComponent<Enemy>();
        if (enemy != null)
        {
            float time = enemyHit(enemy);
            currentWeapon.BehaviorTracker.signalEnemyHit(playerPosition, enemy.transform.position, time);
        }
    }

    private float enemyHit(Enemy enemy)
    {
        float time = enemy.damage(currentWeapon.getCharacteristics()[(int)IWeapon.Stats.DAMAGE], dir);
        foreach (IWeapon w in weapons)
            if (w != null && w.isEvolving)
                w.changeEvolution(currentWeapon == w);

        return time;
    }

    public void setHitParameters(Vector2 playerPosition, Vector2 dir, LayerMask hittableLayers, IWeapon currentWeapon, IWeapon[] weapons)
    {
        this.dir = new Vector2(dir.x, -dir.y);
        this.hittableLayers = hittableLayers;
        this.currentWeapon = currentWeapon;
        this.weapons = weapons;
        area = this.currentWeapon.getCharacteristic(IWeapon.Stats.AOE);
        this.playerPosition = playerPosition;
    }
}
