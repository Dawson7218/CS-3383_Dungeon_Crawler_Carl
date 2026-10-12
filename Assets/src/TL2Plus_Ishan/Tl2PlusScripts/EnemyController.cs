
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

    public int CurrentHP => enemy != null ? enemy.Hp : 0;
    public bool IsDead => enemy != null && enemy.IsDefeated();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        enemy = new Enemy(1, maxHealth, new IdleState());
        currentState = new IdleState();

        if (animator != null)
            animator.Play("Enemy_Idle");
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

        TestPlayerHealth playerHealth =
            player.GetComponent<TestPlayerHealth>();

        if (playerHealth != null && playerHealth.IsDead)
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

        TestPlayerHealth playerHealth =
            player.GetComponent<TestPlayerHealth>();

        if (playerHealth == null || playerHealth.IsDead)
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

        playerHealth.TakeDamage(attackDamage);
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
        }
    }
}
