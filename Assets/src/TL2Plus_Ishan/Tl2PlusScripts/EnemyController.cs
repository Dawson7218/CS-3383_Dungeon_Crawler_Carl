using UnityEngine;
using UnityEngine.Serialization;

// Base class for every enemy. RunCycle() is the template method: the
// patrol / chase / attack logic is the same for all enemies and only
// PerformAttack() (and the animation names) change in the subclasses.
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] protected Transform player;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 12f;

    // Used to be called shootingRange on the drone, keep the old name so
    // the value on the prefab doesn't get lost.
    [SerializeField, FormerlySerializedAs("shootingRange")]
    protected float attackRange = 2f;
    [SerializeField] protected float attackExitBuffer = 0.3f;

    [Header("Patrol (radius 0 = stand still)")]
    [SerializeField] protected float patrolRadius = 3f;
    [SerializeField] protected float patrolSpeedMultiplier = 0.5f;
    [SerializeField] protected float patrolPauseTime = 1.5f;

    [Header("Attack")]
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float attackCooldown = 1.5f;

    [Header("Health")]
    [SerializeField] protected int maxHealth = 50;
    [SerializeField] protected float despawnDelay = 1.5f;

    [Header("Sprite")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected bool spriteFacesRightByDefault = true;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D enemyCollider;
    protected PlayerController playerController;

    private Enemy enemy;

    // Reusable states
    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    private DeadState deadState;
    private EnemyState currentState;

    private string currentAnim;
    private float nextAttackTime;
    private float attackAnimUntil;
    private float patrolWaitUntil;
    private float patrolDeadline;
    private Vector2 spawnPoint;
    private Vector2 patrolTarget;
    private bool defeatReported;

    public static event System.Action<Enemy> OnEnemyDefeated;

    public int CurrentHP => enemy != null ? enemy.Hp : 0;
    public bool IsDead => enemy != null && enemy.IsDefeated();
    protected bool FacingRight { get; private set; } = true;

    // The part each enemy type changes. Default is the basic melee hit.
    protected virtual void PerformAttack()
    {
        Debug.Log("Enemy attacks!");
        DamagePlayer(attackDamage);
    }

    // Animation names, subclasses override these if their clips are
    // named differently.
    protected virtual string IdleAnim => "Enemy_Idle";
    protected virtual string MoveAnim => "Enemy_Run";
    protected virtual string AttackAnim => "Enemy_Attack";
    protected virtual string DeathAnim => "Enemy_Die";
    protected virtual float AttackAnimDuration => 0.6f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        // Get the SpriteRenderer automatically if one is not assigned.
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Create states only once.
        idleState = new IdleState();
        chaseState = new ChaseState();
        attackState = new AttackState();
        deadState = new DeadState();

        enemy = new Enemy(1, maxHealth, idleState);
        currentState = idleState;

        spawnPoint = rb.position;
        patrolTarget = spawnPoint;
    }

    protected virtual void Start()
    {
        // Use the target the spawner gave us, otherwise find the player.
        if (player != null)
            SetTarget(player);
        else
            TryFindPlayer();

        PlayAnimation(IdleAnim);
    }

    private void FixedUpdate()
    {
        RunCycle();
    }

    // Template method. Not virtual on purpose so the order of the steps
    // can't be changed by a subclass.
    public void RunCycle()
    {
        if (IsDead)
            return;

        if (player == null)
            TryFindPlayer();

        if (player == null ||
            (playerController != null && playerController.IsDead))
        {
            Patrol();
            return;
        }

        // Let the attack animation finish first.
        if (Time.time < attackAnimUntil)
        {
            Stop();
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float distance = toPlayer.magnitude;

        FaceTarget(toPlayer.x);

        // Small buffer so the enemy doesn't flicker between attack and
        // chase right at the edge of the range.
        float threshold =
            currentState == attackState
                ? attackRange + attackExitBuffer
                : attackRange;

        if (distance <= threshold)
        {
            ChangeState(attackState);
            Stop();

            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                attackAnimUntil = Time.time + AttackAnimDuration;

                PlayAnimation(AttackAnim, true);
                PerformAttack();
            }
            else
            {
                PlayAnimation(IdleAnim);
            }
        }
        else if (distance <= detectionRange)
        {
            ChangeState(chaseState);

            rb.linearVelocity = toPlayer.normalized * moveSpeed;
            PlayAnimation(MoveAnim);
        }
        else
        {
            Patrol();
        }
    }

    // Wander around the spawn point while the player is out of range.
    private void Patrol()
    {
        ChangeState(idleState);

        if (patrolRadius <= 0.01f || Time.time < patrolWaitUntil)
        {
            Stop();
            PlayAnimation(IdleAnim);
            return;
        }

        Vector2 toPoint = patrolTarget - rb.position;

        // Reached the point (or got stuck on a wall), wait a bit and
        // pick a new one.
        if (toPoint.magnitude <= 0.2f || Time.time > patrolDeadline)
        {
            patrolWaitUntil = Time.time + patrolPauseTime;
            patrolTarget = spawnPoint + Random.insideUnitCircle * patrolRadius;
            patrolDeadline = patrolWaitUntil + 4f;

            Stop();
            PlayAnimation(IdleAnim);
            return;
        }

        rb.linearVelocity =
            toPoint.normalized * moveSpeed * patrolSpeedMultiplier;

        FaceTarget(toPoint.x);
        PlayAnimation(MoveAnim);
    }

    protected void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // Flip the sprite when the facing direction changes.
    protected void FaceTarget(float dx)
    {
        if (spriteRenderer == null || Mathf.Abs(dx) < 0.05f)
            return;

        FacingRight = dx > 0f;
        spriteRenderer.flipX = FacingRight != spriteFacesRightByDefault;
    }

    protected void DamagePlayer(int amount)
    {
        if (playerController != null && !playerController.IsDead)
            playerController.TakeDamage(amount);
    }

    protected void PlayAnimation(string clip, bool restart = false)
    {
        if (animator == null || string.IsNullOrEmpty(clip))
            return;

        if (!restart && currentAnim == clip)
            return;

        currentAnim = clip;

        // The name has to match the Animator state exactly (capital
        // letters too), otherwise Play() does nothing. Warn once and
        // list what the controller actually has.
        if (!animator.HasState(0, Animator.StringToHash(clip)))
        {
            string available = "";

            if (animator.runtimeAnimatorController != null)
            {
                foreach (AnimationClip c in
                    animator.runtimeAnimatorController.animationClips)
                {
                    available += c.name + ", ";
                }
            }

            Debug.LogWarning(
                name + ": Animator has no state named '" + clip +
                "'. Clips in its controller: " + available,
                this
            );
            return;
        }

        animator.Play(clip, 0, 0f);
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        enemy.ChangeState(newState);

        Debug.Log(enemy.Update());
    }

    private void TryFindPlayer()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            SetTarget(obj.transform);
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

        if (IsDead)
            Die();
    }

    protected virtual void Die()
    {
        ChangeState(deadState);
        Stop();

        // So projectiles and the player pass through the corpse.
        if (enemyCollider != null)
            enemyCollider.enabled = false;

        PlayAnimation(DeathAnim, true);

        // Only report the defeat once.
        if (!defeatReported)
        {
            defeatReported = true;
            OnEnemyDefeated?.Invoke(enemy);
        }

        // Keep the object around long enough for the death animation.
        Destroy(gameObject, despawnDelay);
    }
}
