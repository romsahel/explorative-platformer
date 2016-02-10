using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
	private AudioClip enemyDie;
	AudioSource audioSource;

    public static Transform target = null;
    public LayerMask visibleLayers;

    [Header("> Properties")]
    public float health = 100f;
    public int strength = 20;
    public float attackFrequency = 20;
    public float attackRange = 0.4f;

    private float pushBackForce = 40f;

    protected Animator _animator;
    protected Rigidbody2D _body;

    private float timeWhenFirstHit = -1;
    protected float timeToFire = 0;

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("Enemies"));
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("Collectible"));
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("OpenedDoors"));
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("Void"));

		enemyDie = Resources.Load ("Audio/enemyDie") as AudioClip;

        if (target == null)
        {
            target = GameObject.FindObjectOfType<Player>().transform;
            FairyProjectile.target = target;
            FairyProjectile.visibleLayers = visibleLayers;
        }
    }

    protected bool attemptAttackTarget()
    {
        if (Vector2.Distance(target.transform.position, _body.position) < 0.4f && Time.time > timeToFire)
        {
            timeToFire = Time.time + 1 / attackFrequency;
            target.GetComponent<Player>().damage(strength);
            return true;
        }
        return false;
    }

    public virtual float damage(float damage, Vector2 dir)
    {
        if (health <= 0)
            return 0;

        if (timeWhenFirstHit == -1)
            timeWhenFirstHit = Time.time;

        health -= damage;
        if (_animator.HasState(0, Animator.StringToHash("Hit")))
            _animator.Play("Hit");

        _body.AddForce(dir * pushBackForce, ForceMode2D.Force);

        if (health <= 0)
            return kill();
        return 0;
    }

    protected virtual float kill()
    {

		audioSource = GetComponent<AudioSource>();
        _animator.Play("Dying");
        _body.gravityScale = 1;
        //this._body.gameObject.SetActive(false);
        //this.GetComponent<Collider2D>().enabled = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.01f);
        this.gameObject.layer = LayerMask.NameToLayer("Void");
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), target.GetComponent<Collider2D>());
        this.enabled = false;

		audioSource.PlayOneShot (enemyDie, 0.1F);

        return Mathf.Max(Time.time - timeWhenFirstHit, 0.001f);
    }

}