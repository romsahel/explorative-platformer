using UnityEngine;
using Pathfinding;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : Enemy
{
    // How many times per second we update the path 
    public float updateRate = 2f;

    private Seeker _seeker;

    // calculated path
    public Path path;

    [Header("> Enemy movements")]
    // speed per second
    public float chaseSpeed = 100f;
    public float patrolSpeed = 25f;

    [Header("> Patrolling settings")]
    public float patrolDistanceRight = 0.25f;
    public float patrolDistanceLeft = 0.25f;
    public float sightDistance = 1f;

    [Header("> Misc")]
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded;

    // Max distance from the AI to a waypoint for it to continue to the next
    public float nextWaypointDistance = 0.5f;
    private int currentWaypoint = 0;

    private bool facingLeft = true;
    private bool chasing = false;
    private Vector3[] destinations;
    private int destinationIndex;
    private int chaseTimer;
    private int maxChasingTime = 10 * 1000;
    private float gravityProgress = 0f;


    new void Awake()
    {
        base.Awake();
        chaseTimer = maxChasingTime;
        _seeker = GetComponent<Seeker>();

        if (target == null)
            Debug.LogError("No player found.");

        destinationIndex = 0;
        destinations = new Vector3[] {
            this.transform.position + Vector3.right * patrolDistanceRight,
            this.transform.position + Vector3.left * patrolDistanceLeft
        };
        if (transform.localScale.x < 0)
            facingLeft = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.01f);
    }

    void FixedUpdate()
    {
        _body.gravityScale = 0;
        chaseOrPatrol();
    }

    private void chaseOrPatrol()
    {
        if (health <= 0)
            return;

        if (chaseTimer < maxChasingTime)
        {
            chase();
            return;
        }

        StopCoroutine(UpdatePath());
        chasing = false;
        if (canEnemyHitTarget(Mathf.Sign(_body.velocity.x) * Vector2.right))
        {
            chaseTimer = 0;
            chasing = true;
            StartCoroutine(UpdatePath());
            chase();
        }
        else
            patrol();
    }

    private bool canEnemyHitTarget(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position,
                                         0.12f, direction,
                                         sightDistance,
                                         visibleLayers);

        return (hit.collider != null && hit.transform == target.transform);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            this.path = p;
            currentWaypoint = 0;
        }
    }

    private IEnumerator UpdatePath()
    {
        _seeker.StartPath(transform.position, target.position, OnPathComplete);
        yield return new WaitForSeconds(1 / updateRate);
        if (health > 0)
            StartCoroutine(UpdatePath());
    }

    void chase()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;
            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        if (Vector2.Distance(target.position, transform.position) < attackRange
            && canEnemyHitTarget(target.position - transform.position))
        {
            gravityProgress += Time.deltaTime * 0.1f;
            _body.gravityScale = Mathf.Lerp(0, 0.5f, gravityProgress);
            if (Time.time > timeToFire)
            {
                if (_animator.HasState(0, Animator.StringToHash("Shoot")))
                {
                    Transform projectile = this.transform.FindChild("trail");
                    if (projectile != null)
                    {
                        Transform newProjectile = (Transform)Instantiate(projectile, projectile.position, projectile.rotation);
                        newProjectile.gameObject.SetActive(true);
                        newProjectile.GetComponent<FairyProjectile>().setStrength(strength);
                        newProjectile.parent = projectile.parent;
                    }
                    _animator.Play("Shoot");
                }
                else
                    target.GetComponent<Player>().damage(strength);
                timeToFire = Time.time + 1 / attackFrequency;
            }
            flipSprite(target.position.x - transform.position.x);
            return;
        }
        gravityProgress = 0f;
        // Direction to next waypoint
        move(path.vectorPath[currentWaypoint], chaseSpeed);

        if (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            currentWaypoint++;

        flipSprite(_body.velocity.x);
    }


    private void patrol()
    {
        if (Mathf.Abs(transform.position.x - destinations[destinationIndex].x) < nextWaypointDistance)
            destinationIndex = (destinationIndex + 1) % 2;

        move(destinations[destinationIndex], patrolSpeed);
        flipSprite(_body.velocity.x);
    }


    private void move(Vector3 destination, float speed)
    {
        Vector2 dir = (destination - transform.position);
        dir = dir.normalized * speed * Time.fixedDeltaTime;
        _body.AddForce(dir, fMode);
    }

    private void flipSprite(float dirX)
    {
        if (dirX > 0 && facingLeft)
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            facingLeft = false;
        }
        else if (dirX < 0 && !facingLeft)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            facingLeft = true;
        }
    }

    public override float damage(float damage, Vector2 dir)
    {
        if (!chasing)
        {
            chaseTimer = 0;
            chasing = true;
            StartCoroutine(UpdatePath());
        }
        return base.damage(damage, dir);
    }

    #region OnDrawGizmosSelected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 from = new Vector2(this.transform.position.x - patrolDistanceLeft, this.transform.position.y - 0.15f);
        Vector2 to = new Vector2(this.transform.position.x + patrolDistanceRight, this.transform.position.y - 0.15f);
        Gizmos.DrawLine(from, to);
        from = new Vector2(this.transform.position.x - patrolDistanceLeft, this.transform.position.y + 0.15f);
        to = new Vector2(this.transform.position.x + patrolDistanceRight, this.transform.position.y + 0.15f);
        Gizmos.DrawLine(from, to);
        from = new Vector2(this.transform.position.x, this.transform.position.y + 0.1f);
        to = new Vector2(this.transform.position.x - sightDistance, this.transform.position.y + 0.1f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(from, to);
        from = new Vector2(this.transform.position.x, this.transform.position.y - 0.05f);
        to = new Vector2(this.transform.position.x - attackRange, this.transform.position.y - 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, to);
    }
    #endregion
}
