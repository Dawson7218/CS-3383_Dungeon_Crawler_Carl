
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
    [SerializeField] private bool spriteFacesRightByDefault = true;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D enemyCollider;

    private PlayerController playerController;
    private Enemy enemy;

    // Reusable states
    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    private DeadState deadState;

    private EnemyState currentState;

    private float nextAttackTime;
    private float initialScaleX;

    private bool defeatReported;

    public int CurrentHP => enemy != null ? enemy.Hp : 0;
    public bool IsDead => enemy != null && enemy.IsDefeated();

    public static event System.Action<Enemy> OnEnemyDefeated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        initialScaleX = Mathf.Abs(transform.localScale.x);

        // Create states only once.
        idleState = new IdleState();
        chaseState = new ChaseState();
        attackState = new AttackState();
        deadState = new DeadState();

        enemy = new Enemy(1, maxHealth, idleState);

        currentState = idleState;

        if (animator != null)
            animator.Play("Enemy_Idle");
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                SetTarget(playerObj.transform);
        }
        else
        {
            SetTarget(player);
        }
    }

    private void FixedUpdate()
    {
        if (IsDead)
        {
            ChangeState(deadState);
            return;
        }

        if (player == null || playerController == null ||
            playerController.IsDead)
        {
            ChangeState(idleState);
            return;
        }

        FacePlayer();

        Vector2 direction =
            (Vector2)player.position - rb.position;

        float distanceSqr = direction.sqrMagnitude;

        float attackThreshold =
            currentState == attackState
                ? attackRange + attackExitBuffer
                : attackRange;

        float attackThresholdSqr =
            attackThreshold * attackThreshold;

        float detectionRangeSqr =
            detectionRange * detectionRange;

        if (distanceSqr <= attackThresholdSqr)
        {
            ChangeState(attackState);

            // Only deal damage inside the actual attack range.
            if (distanceSqr <= attackRange * attackRange &&
                Time.time >= nextAttackTime)
            {
                PerformAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else if (distanceSqr <= detectionRangeSqr)
        {
            ChangeState(chaseState);

            direction.Normalize();

            Vector2 newPosition =
                rb.position +
                direction * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);
        }
        else
        {
            ChangeState(idleState);
        }
    }

    private void FacePlayer()
    {
        float dx = player.position.x - transform.position.x;

        if (Mathf.Abs(dx) < 0.05f)
            return;

        bool shouldFaceRight = dx > 0f;

        float sign =
            shouldFaceRight == spriteFacesRightByDefault
                ? 1f
                : -1f;

        Vector3 scale = transform.localScale;
        scale.x = initialScaleX * sign;

        transform.localScale = scale;
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        enemy.ChangeState(newState);

        if (animator != null)
        {
            if (newState == idleState)
                animator.Play("Enemy_Idle", 0, 0f);

            else if (newState == chaseState)
                animator.Play("Enemy_Run", 0, 0f);

            else if (newState == attackState)
                animator.Play("Enemy_Attack", 0, 0f);

            else if (newState == deadState)
                animator.Play("Enemy_Die", 0, 0f);
        }

        Debug.Log(enemy.Update());
    }

    private void PerformAttack()
    {
        if (playerController == null || IsDead ||
            playerController.IsDead)
            return;

        Debug.Log("Enemy attacks!");

        playerController.TakeDamage(attackDamage);
    }

    private void ReportEnemyDefeat()
    {
        if (!IsDead || defeatReported)
            return;

        defeatReported = true;

        OnEnemyDefeated?.Invoke(enemy);

        Debug.Log("Enemy defeat reported.");
    }

    public void SetTarget(Transform target)
    {
        player = target;

        playerController = player != null
            ? player.GetComponent<PlayerController>()
            : null;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        enemy.TakeDamage(damage);

        Debug.Log("Enemy HP: " + enemy.Hp);

        if (IsDead)
        {
            ChangeState(deadState);

            if (enemyCollider != null)
                enemyCollider.enabled = false;

            rb.linearVelocity = Vector2.zero;

            ReportEnemyDefeat();
        }
    }
}
