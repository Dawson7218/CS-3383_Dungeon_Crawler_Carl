using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossMinionAttack : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float orbitRadius = 2.5f;
    [SerializeField] private float orbitSpeed = 18f;

    [Header("Attack")]
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float fireOffset = 0.5f;

    private EnemyController health;
    private Rigidbody2D body;
    private Transform target;
    private BossProjectile projectilePrefab;

    private float orbitAngle;
    private float fireTimer;
    private bool initialized;

    private readonly List<BossProjectile> projectiles =
        new List<BossProjectile>();

    private void Awake()
    {
        health = GetComponent<EnemyController>();
        body = GetComponent<Rigidbody2D>();

        // Keep EnemyController's health methods, but stop its
        // normal chasing and melee-attack update.
        health.enabled = false;

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.linearVelocity = Vector2.zero;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Initialize(
        Transform newTarget,
        BossProjectile newProjectilePrefab,
        float startingAngle,
        float firstShotDelay)
    {
        target = newTarget;
        projectilePrefab = newProjectilePrefab;
        orbitAngle = startingAngle;
        fireTimer = firstShotDelay;
        initialized = true;
    }

    private void FixedUpdate()
    {
        if (!initialized || health.IsDead || target == null)
            return;

        orbitAngle += orbitSpeed * Time.fixedDeltaTime;

        float radians = orbitAngle * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)) * orbitRadius;

        Vector2 desiredPosition = (Vector2)target.position + offset;

        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            desiredPosition,
            moveSpeed * Time.fixedDeltaTime);

        body.MovePosition(nextPosition);
    }

    private void Update()
    {
        if (!initialized)
            return;

        if (health.IsDead)
        {
            initialized = false;
            ClearProjectiles();
            Destroy(gameObject);
            return;
        }

        if (target == null || Time.deltaTime <= 0f)
            return;

        fireTimer -= Time.deltaTime;

        if (fireTimer > 0f)
            return;

        FireOneProjectile();
        fireTimer = Mathf.Max(0.1f, fireInterval);
    }

    private void FireOneProjectile()
    {
        if (projectilePrefab == null)
            return;

        Vector2 direction =
            (Vector2)target.position - body.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        Vector3 spawnPosition = transform.position
            + (Vector3)(direction * fireOffset);

        BossProjectile projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity);

        projectile.Initialize(direction);

        projectiles.RemoveAll(item => item == null);
        projectiles.Add(projectile);
    }

    private void ClearProjectiles()
    {
        foreach (BossProjectile projectile in projectiles)
        {
            if (projectile != null)
                Destroy(projectile.gameObject);
        }

        projectiles.Clear();
    }

    private void OnDisable()
    {
        ClearProjectiles();
    }
}