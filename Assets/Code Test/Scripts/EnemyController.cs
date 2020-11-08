using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private bool facingRight;

    private Rigidbody2D rigidBody2D;

    private Vector2 velocity;

    #endregion

    [SerializeField]
    private float speed = 1.0f;

    private Animator animator;


    private bool attack;
    private bool attackComplete;

    private bool patrol;
    public GameObject[] patrolPoints;
    private int patrolPointIndex;
    private GameObject target;
    private bool targettingPlayer;

    [SerializeField]
    private float attackRadius = 1.0f;

    [SerializeField]
    private float attackDamage = 1.0f;

    [SerializeField]
    private float health = 10.0f;

    bool alive;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    bool deathComplete;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

    }

    public void Start()
    {
        attack = false;
        attackComplete = false;

        patrol = true;
        patrolPointIndex = 0;
        target = patrolPoints[patrolPointIndex];
        patrolPointIndex++;

        alive = true;
        deathComplete = false;
    }

    private void FixedUpdate()
    {
        // ------------ Health check ------------
        if (health <= 0.0f)
        {
            alive = false;
            animator.SetBool("Alive", alive);
        }

        if(!alive)
        {
            if(deathComplete)
            {
                GameUI.Instance.ShowMessage("[ LEVEL COMPLETE ]", "GREAT JOB", Color.green, 0.0f);
                Destroy(gameObject);
            }
            return;
        }

        // -------- Attacking ---------
        if (animator.GetBool("Attacking") == true)
        {
            //
        }
        else
        {
            if (target)
            {
                float horizontalInput = 0.0f;
                float distanceToTarget = Mathf.Abs(transform.position.x - target.transform.position.x);
                if (target.tag == "Player")
                {
                    Player player = target.GetComponentInParent<Player>();
                    if(!player.IsAlive())
                    {
                        target = patrolPoints[patrolPointIndex];
                    }
                    else if(distanceToTarget < attackRadius)
                    {
                        // Attack!
                        animator.SetBool("Attacking", true);
                        attackComplete = false;
                    }
                    else
                    {
                        horizontalInput = (target.transform.position.x < transform.position.x) ? -speed : speed;
                    }
                }
                else if(distanceToTarget < 0.1f)
                {
                    target = patrolPoints[patrolPointIndex];
                    patrolPointIndex++;
                    if (patrolPointIndex == patrolPoints.Length)
                    {
                        patrolPointIndex = 0;
                    }
                }
                else
                {
                    horizontalInput = (target.transform.position.x < transform.position.x) ? -speed : speed;
                }

                // -------- Horizontal movement ---------
                Vector2 targetVelocity = new Vector2(horizontalInput * Time.fixedDeltaTime, rigidBody2D.velocity.y);
                if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
                {
                    FlipXScale();
                }

                // ------------ Apply ------------
                rigidBody2D.velocity = targetVelocity;

                // ----- Animation States --------
                animator.SetFloat("Speed", Mathf.Abs(horizontalInput / speed));
            }
        }

        if (attackComplete)
        {
            animator.SetBool("Attacking", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            target = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            target = patrolPoints[patrolPointIndex];
        }
    }

    private void OnAttackAnimationComplete()
    {
        attackComplete = true;
    }

    private void OnAttackHit()
    {
        if(target && target.tag == "Player")
        {
            Player player = target.GetComponentInParent<Player>();
            player.TakeHit(attackDamage);
        }
    }

    public void OnDeathAnimationComplete()
    {
        deathComplete = true;
    }

    public void TakeHit(float damage)
    {
        if (alive)
        {
            StartCoroutine(HitEffect());
            health -= damage;
        }
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.color = Color.white;
    }

    private void FlipXScale()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-1.0f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
