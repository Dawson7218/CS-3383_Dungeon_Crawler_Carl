using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class GruntEnemyController : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 8f;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public int maxHealth = 50;

    [Header("Exact Animation Clip Names")]
    public string idleAnim = "gruntIdle";
    public string runAnim = "gruntRun";
    public string attackAnim = "gruntAttack";
    public string dieAnim = "gruntDie";

    private int currentHealth;
    private float lastAttackTime;
    private float attackAnimTimer = 0f;
    private bool isDead = false;
    private string currentAnimState = "";

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Wait for attack animation to finish before switching back to run/idle
        if (attackAnimTimer > 0f)
        {
            attackAnimTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Flip sprite to face the player
        if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false;

        if (distanceToPlayer <= attackRange)
        {
            // Stop and attack
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                attackAnimTimer = 0.6f; // Plays attack clip for 0.6 seconds
                ChangeAnimationState(attackAnim, true);
            }
            else
            {
                ChangeAnimationState(idleAnim);
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Chase the player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            ChangeAnimationState(runAnim);
        }
        else
        {
            // Stand idle
            rb.linearVelocity = Vector2.zero;
            ChangeAnimationState(idleAnim);
        }
    }

    // Plays an animation clip directly by name without Animator transition arrows
    private void ChangeAnimationState(string newState, bool forceRestart = false)
    {
        if (!forceRestart && currentAnimState == newState) return;

        animator.Play(newState, 0, 0f);
        currentAnimState = newState;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        ChangeAnimationState(dieAnim, true);
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1.5f);
    }
}