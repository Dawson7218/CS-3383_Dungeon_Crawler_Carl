
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RangedEnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float shootingRange = 6f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextAttackTime;
    private string currentAnimation;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        PlayAnimation("Drone_Idle");
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            PlayAnimation("Drone_Idle");
            return;
        }

        if (distance > shootingRange)
        {
            Vector2 direction =
                ((Vector2)player.position - rb.position).normalized;

            rb.linearVelocity = direction * moveSpeed;
            PlayAnimation("Drone_Move");
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || player == null)
            return;

        PlayAnimation("Drone_Attack", true);

        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        DroneProjectile laser =
            projectile.GetComponent<DroneProjectile>();

        if (laser != null)
        {
            Vector2 direction =
                ((Vector2)player.position - (Vector2)spawnPosition).normalized;

            laser.Initialize(direction);
        }
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        PlayAnimation("Drone_Death", true);

        Debug.Log("Ranged enemy defeated!");

        // Keep the GameObject alive for its death animation.
    }

    private void PlayAnimation(string animationName, bool restart = false)
    {
        if (animator == null)
            return;

        if (!restart && currentAnimation == animationName)
            return;

        animator.Play(animationName, 0, 0f);
        currentAnimation = animationName;
    }
}
