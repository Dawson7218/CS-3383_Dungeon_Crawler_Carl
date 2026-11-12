using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackExitBuffer = 0.3f;
    [SerializeField] private int maxHealth = 100;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime;

    private Enemy enemy;
    private EnemyState currentState;
    private Rigidbody2D rb;
    private Animator animator;
    private bool defeatReported = false;

    public int CurrentHP => enemy != null ? enemy.Hp : 0;
    public bool IsDead => enemy != null && enemy.IsDefeated();
    public static event System.Action<Enemy> OnEnemyDefeated;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        enemy = new Enemy(1, maxHealth, new IdleState());
        currentState = new IdleState();

        if (animator != null)
            animator.Play("Enemy_Idle");
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (enemy.IsDefeated())
        {
            ChangeState(new DeadState());
            return;
        }

        if (player == null)
        {
            ChangeState(new IdleState());
            return;
        }

        PlayerController playerController =
            player.GetComponent<PlayerController>();

        if (playerController != null && playerController.IsDead)
        {
            ChangeState(new IdleState());
            return;
        }

        float distance = Vector2.Distance(
            rb.position,
            (Vector2)player.position
        );

        // Prevent rapid switching between Chase and Attack.
        float attackThreshold = currentState is AttackState
            ? attackRange + attackExitBuffer
            : attackRange;

        if (distance <= attackThreshold)
        {
            ChangeState(new AttackState());

            if (Time.time >= nextAttackTime)
            {
                PerformAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else if (distance <= detectionRange)
        {
            ChangeState(new ChaseState());

            Vector2 direction =
                ((Vector2)player.position - rb.position).normalized;

            Vector2 newPosition =
                rb.position + direction * moveSpeed *
                Time.fixedDeltaTime;

            rb.MovePosition(newPosition);
        }
        else
        {
            ChangeState(new IdleState());
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState.GetType() == newState.GetType())
            return;

        currentState = newState;
        enemy.ChangeState(newState);

        if (animator != null)
        {
            if (newState is IdleState)
                animator.Play("Enemy_Idle", 0, 0f);

            else if (newState is ChaseState)
                animator.Play("Enemy_Run", 0, 0f);

            else if (newState is AttackState)
                animator.Play("Enemy_Attack", 0, 0f);

            else if (newState is DeadState)
                animator.Play("Enemy_Die", 0, 0f);
        }

        Debug.Log(enemy.Update());
    }

    private void PerformAttack()
    {
        if (player == null || enemy.IsDefeated())
            return;

        PlayerController playerController =
            player.GetComponent<PlayerController>();

        if (playerController == null || playerController.IsDead)
            return;

        // Confirm the player is still within attack range.
        float distance = Vector2.Distance(
            rb.position,
            (Vector2)player.position
        );

        if (distance > attackRange + attackExitBuffer)
            return;

        Debug.Log("Goblin attacks!");

        if (animator != null)
            animator.Play("Enemy_Attack", 0, 0f);

        playerController.TakeDamage(attackDamage);
    }

    private void ReportEnemyDefeat()
    {
        // Prevent duplicate defeat notifications.
        if (enemy == null || !enemy.IsDefeated() || defeatReported)
            return;

        // Mark the enemy as reported.
        defeatReported = true;

        // Notify other systems that this enemy was defeated.
        OnEnemyDefeated?.Invoke(enemy);

        Debug.Log("Enemy defeat reported.");
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void TakeDamage(int damage)
    {
        if (enemy.IsDefeated())
            return;

        enemy.TakeDamage(damage);

        Debug.Log("Goblin HP: " + enemy.Hp);

        if (enemy.IsDefeated())
        {
            ChangeState(new DeadState());

            // Prevent further collisions after death.
            Collider2D enemyCollider =
                GetComponent<Collider2D>();

            if (enemyCollider != null)
                enemyCollider.enabled = false;

            rb.linearVelocity = Vector2.zero;

            // Report the enemy defeat exactly once.
            ReportEnemyDefeat();
        }
    }
}