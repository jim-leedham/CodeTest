using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    private Rigidbody2D rigidBody2D;
    private Animator animator;

    [SerializeField] private bool facingRight;
    [SerializeField] private float speed = 1.0f;
    private float horizontalInput;

    [SerializeField] private bool onGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 5.6f;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpActivateForce;
    [SerializeField] private float jumpBoostMax = 0.35f;
    private bool jump;
    private bool jumpInput;
    private bool jumping;
    private int jumps;
    private int maxJumps;
    private float jumpBoost;

    [SerializeField] private Transform wallGrabPoint;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckRadius = 1.0f;
    private bool isGrabbingWall;
    private float gravityScale;

    private bool inputEnabled;
    private float inputCooldown;

    [SerializeField] private float attackDamage;
    private bool attack;
    private bool attackComplete;

    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    private bool alive;
    private bool revive;
    private bool reviveComplete;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float enemyRadiusCheck;

    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip takeDamageAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip attackHitAudioClip;

    private List<AbilityType> abilities;
    private Dictionary<AbilityType, bool> unlockedAbilities;

    [SerializeField] private Collider2D[] colliders;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        unlockedAbilities = new Dictionary<AbilityType, bool>();
    }

    private void Start()
    {
        jumping = false;
        jumps = 0;
        maxJumps = 1;
        gravityScale = rigidBody2D.gravityScale;
        attack = false;
        attackComplete = false;
        alive = true;
        health = maxHealth;
        GameUI.Instance.UpdateHealthBar(health, maxHealth);
        animator.SetBool("Alive", alive);
        revive = false;
        reviveComplete = false;
        for (int i = 0; i < (int)AbilityType.COUNT; i++)
        {
            unlockedAbilities[(AbilityType)i] = false;
        }
    }

    void Update()
    {
        // Continous inputs
        horizontalInput = Input.GetAxisRaw("Horizontal") * speed;
        jumpInput = Input.GetButton("Jump");

        // One-time inputs
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if(Input.GetButtonDown("Attack"))
        {
            attack = true;
        }
        if (Input.GetButtonDown("Revive"))
        {
            revive = true;
        }
    }

    private void FixedUpdate()
    {
        // ------------ Health check ------------
        if (alive && health <= 0.0f)
        {
            alive = false;
            animator.SetBool("Alive", alive);
            audioSource.clip = deathAudioClip;
            audioSource.Play();
            rigidBody2D.gravityScale = 0.0f;
            foreach (Collider2D collider2d in colliders)
            {
                collider2d.enabled = false;
            }
        }

        if (!alive)
        {
            if(inputEnabled && revive)
            {
                foreach (Collider2D collider2d in colliders)
                {
                    collider2d.enabled = true;
                }
                rigidBody2D.gravityScale = gravityScale;
                animator.SetBool("Reviving", true);
                revive = false;
                reviveComplete = false;
                inputEnabled = false;
            }

            if(reviveComplete)
            {
                reviveComplete = false;
                health = maxHealth;
                GameUI.Instance.UpdateHealthBar(health, maxHealth);
                alive = true;
                animator.SetBool("Reviving", false);
                animator.SetBool("Alive", true);
                inputEnabled = true;
            }
            else
            {
                return;
            }
        }

        // ------------ Ground check ------------
        bool wasOnGround = onGround;
        onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!wasOnGround && onGround)
        {
            animator.SetBool("Landing", true);
            inputEnabled = false; // we'll re-enable in OnLandAnimationComplete
            jumping = false;
            jumps = 0;
        }

        if (inputEnabled)
        {
            // -------- Attacking ---------
            if (attack && unlockedAbilities[AbilityType.ATTACK] && animator.GetBool("Attacking") == false)
            {
                animator.SetBool("Attacking", true);
                animator.SetFloat("AttackID", Random.Range(0, 2) / 2); // randomly pick from our 3 attack anims
                attack = false;
                attackComplete = false;

                audioSource.clip = attackHitAudioClip;
                audioSource.Play();
            }

            // -------- Horizontal movement ---------
            Vector2 targetVelocity = new Vector2(horizontalInput * Time.fixedDeltaTime, rigidBody2D.velocity.y);
            if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
            {
                FlipXScale();
            }

            // ------------ Wall grabbing -----------
            if (unlockedAbilities[AbilityType.WALLGRAB])
            {
                isGrabbingWall = false;
                bool canGrabWall = Physics2D.OverlapCircle(wallGrabPoint.position, wallCheckRadius, wallLayer);

                if (canGrabWall && !onGround)
                {
                    if ((facingRight && horizontalInput > 0.0f) ||
                        (!facingRight && horizontalInput < 0.0f))
                    {
                        isGrabbingWall = true;
                    }
                }

                if (isGrabbingWall)
                {
                    targetVelocity = Vector2.zero;
                    rigidBody2D.gravityScale = 0.0f;
                    jumps = 0;

                    if (jump)
                    {
                        DisableInput(0.1f);
                        FlipXScale();
                        targetVelocity.x = -horizontalInput * Time.fixedDeltaTime;
                        rigidBody2D.gravityScale = gravityScale;
                        isGrabbingWall = false;
                    }
                }
                else
                {
                    rigidBody2D.gravityScale = gravityScale;
                }
            }

            // ------------ Jumping ------------
            if (jump)
            {
                if(jumps < maxJumps)
                {
                    targetVelocity.y = jumpActivateForce;
                    onGround = false;
                    jumpBoost = 0.0f;
                    jumping = true;
                    jumps++;
                    audioSource.clip = jumpAudioClip;
                    audioSource.Play();
                }
                jump = false;
            }
            else if (jumping)
            {
                if (jumpInput && jumpBoost < jumpBoostMax)
                {
                    targetVelocity.y += jumpForce;
                    jumpBoost += Time.fixedDeltaTime;
                }
            }

            // ------------ Apply ------------
            rigidBody2D.velocity = targetVelocity;
        }
        else
        {
            inputCooldown -= Time.fixedDeltaTime;
            if(inputCooldown < 0.0f)
            {
                inputEnabled = true;
            }
        }

        // ---------- Animator update ----------
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput / speed));
        animator.SetFloat("YVelocity", rigidBody2D.velocity.y);
        animator.SetBool("Ground", onGround);
        animator.SetBool("Squatting", false);
        animator.SetBool("Hanging", isGrabbingWall);

        if(attackComplete)
        {
            animator.SetBool("Attacking", false);
        }
        
    }

    private void OnAttackAnimationComplete()
    {
        attackComplete = true;
    }

    private void OnLandAnimationComplete()
    {
        animator.SetBool("Landing", false);
        inputEnabled = true;
    }

    private void OnReviveAnimationComplete()
    {
        reviveComplete = true;
    }

    private void DisableInput(float time)
    {
        inputEnabled = false;
        inputCooldown = time;
    }

    private void FlipXScale()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-1.0f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void TakeHit(float damage)
    {
        if(alive)
        {
            StartCoroutine(HitEffect());
            health -= damage;
            audioSource.clip = takeDamageAudioClip;
            audioSource.Play();
            GameUI.Instance.UpdateHealthBar(health, maxHealth);
        }
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.color = Color.white;
    }

    private void OnAttackHit()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(wallGrabPoint.position, enemyRadiusCheck, enemyLayer);
        foreach(Collider2D enemyCollider in enemyColliders)
        {
            if (!enemyCollider.isTrigger)
            {
                EnemyController enemy = enemyCollider.gameObject.GetComponent<EnemyController>();
                enemy.TakeHit(attackDamage);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PickUp")
        {
            PickUp pickUp = collision.GetComponent<PickUp>();
            if(pickUp.Type == PickUpType.ABILITY)
            {
                AbilityPickUp abilityPickUp = collision.GetComponent<AbilityPickUp>();
                if(abilityPickUp)
                {
                    AbilityType type = abilityPickUp.AbilityType;
                    UnlockAbility(type);
                    abilityPickUp.Consume();
                }
            }
        }
    }

    private void UnlockAbility(AbilityType type)
    {
        unlockedAbilities[type] = true;
        string title = "[ ABILITY UNLOCKED ]";
        string message = "";
        switch(type)
        {
            case AbilityType.WALLGRAB:
                message = "WALL GRAB";
                break;
            case AbilityType.DOUBLEJUMP:
                maxJumps = 2;
                message = "DOUBLE JUMP";
                break;
            case AbilityType.ATTACK:
                message = "ATTACK";
                break;
        }

        GameUI.Instance.ShowMessage(title, message, Color.red, 3f);
    }

    public bool IsAlive()
    {
        return alive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(wallGrabPoint.position, wallCheckRadius);
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(wallGrabPoint.position, enemyRadiusCheck);
    }
}
