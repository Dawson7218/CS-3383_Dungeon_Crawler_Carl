using System;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private int maximumHealth = 1000;
    [SerializeField] private bool startAutomatically = false;

    [Header("Attack References")]
    [SerializeField] private BossProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;

    [Header("Boss Visual")]
    [SerializeField] private SpriteRenderer bossVisual;

    [Header("Boss Movement")]
    [SerializeField] private float moveSpeed = 0.8f;
    [SerializeField] private float orbitRadius = 4f;
    [SerializeField] private float orbitSpeed = 12f;

    [Header("Ranged Barrage")]
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float spreadAngle = 15f;

    [Header("Minion Summoning")]
    [SerializeField] private BossMinionSpawner minionSpawner;

    [Header("Enrage")]
    [SerializeField] private float enrageInterval = 1f;
    [SerializeField] private int enrageProjectileCount = 8;
    [SerializeField] private float enrageRotationPerBurst = 22.5f;

    private int currentHealth;
    private float attackTimer;
    private float enrageAngle;
    private float movementAngle;

    private bool encounterActive;
    private bool defeatSignaled;
    private bool minionWaveSpawned;
    private bool minionWaveDefeated;

    private BossPhase chainHead;
    private BossPhase activePhase;

    private readonly List<BossProjectile> projectiles =
        new List<BossProjectile>();

    public event Action OnBossDefeated;

    public bool MinionWaveDefeated => minionWaveDefeated;

    private void Start()
    {
        if (startAutomatically)
            StartEncounter();
    }

    public void StartEncounter()
    {
        if (encounterActive || defeatSignaled)
            return;

        if (bossVisual == null)
            bossVisual = GetComponentInChildren<SpriteRenderer>();

        if (maximumHealth < 10 ||
            projectilePrefab == null ||
            firePoint == null ||
            target == null ||
            bossVisual == null ||
            minionSpawner == null ||
            !minionSpawner.IsReady)
        {
            Debug.LogError(
                "Boss setup invalid. Check health, projectile, FirePoint, " +
                "target, boss visual, and minion prefab.",
                this);

            return;
        }

        currentHealth = maximumHealth;
        minionWaveSpawned = false;
        minionWaveDefeated = false;
        enrageAngle = 0f;

        Vector2 offset =
            (Vector2)transform.position - (Vector2)target.position;

        movementAngle =
            Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        chainHead = new Phase1_RangedBarrage();

        BossPhase phase2 = new Phase2_SummonMinions();
        BossPhase phase3 = new Phase3_Enrage();

        chainHead.SetNext(phase2);
        phase2.SetNext(phase3);

        encounterActive = true;
        bossVisual.enabled = true;
        activePhase = null;

        SelectPhase();
    }

    private void Update()
    {
        if (!encounterActive || Time.deltaTime <= 0f)
            return;

        MoveBoss();

        if (activePhase is Phase2_SummonMinions)
        {
            if (minionWaveSpawned &&
                minionSpawner.LivingMinionCount == 0)
            {
                CompleteMinionWave();
            }

            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            return;

        activePhase.ExecutePhase(this);
        attackTimer = GetAttackInterval();
    }

    private void MoveBoss()
    {
        if (target == null)
            return;

        movementAngle += orbitSpeed * Time.deltaTime;
        float radians = movementAngle * Mathf.Deg2Rad;

        Vector3 desiredPosition = target.position + new Vector3(
            Mathf.Cos(radians) * orbitRadius,
            Mathf.Sin(radians) * orbitRadius,
            0f);

        desiredPosition.z = transform.position.z;

        transform.position = Vector3.MoveTowards(
            transform.position,
            desiredPosition,
            moveSpeed * Time.deltaTime);
    }

    private void SelectPhase()
    {
    Debug.Log(
        $"[Boss Check] Object={name}, HP={currentHealth}/{maximumHealth}, " +
        $"WaveDefeated={minionWaveDefeated}",
        this);

    BossPhase selectedPhase = chainHead.Handle(this);

    Debug.Log(
        $"[Boss Check] Selected={selectedPhase?.PhaseName ?? "None"}, " +
        $"Previous={activePhase?.PhaseName ?? "None"}",
        this);

        if (selectedPhase == null)
        {
            Debug.LogError("No matching boss phase.", this);
            encounterActive = false;
            ClearProjectiles();
            minionSpawner.ClearMinions();
            bossVisual.enabled = true;
            return;
        }

        if (selectedPhase == activePhase)
            return;

        activePhase = selectedPhase;
        attackTimer = GetAttackInterval();

        Debug.Log("Boss phase changed: " + activePhase.PhaseName, this);

        if (activePhase is Phase2_SummonMinions)
        {
            ClearProjectiles();
            bossVisual.enabled = false;

            // Execute the summon once when entering Phase 2.
            activePhase.ExecutePhase(this);
        }
        else
        {
            bossVisual.enabled = true;
        }
    }

    public void FireSpread()
    {
        if (!encounterActive || defeatSignaled)
            return;

        Vector2 aimDirection = Vector2.down;

        if (target != null)
        {
            Vector2 offset =
                (Vector2)target.position - (Vector2)firePoint.position;

            if (offset.sqrMagnitude > 0.001f)
                aimDirection = offset.normalized;
        }

        projectiles.RemoveAll(projectile => projectile == null);

        for (int i = -1; i <= 1; i++)
        {
            Vector2 direction =
                Quaternion.Euler(0f, 0f, i * spreadAngle)
                * (Vector3)aimDirection;

            SpawnProjectile(direction);
        }
    }

    public void FireRadialBurst()
    {
        if (!encounterActive || defeatSignaled)
            return;

        projectiles.RemoveAll(projectile => projectile == null);

        int count = Mathf.Max(1, enrageProjectileCount);
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle =
                (enrageAngle + i * angleStep) * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle));

            SpawnProjectile(direction);
        }

        enrageAngle = Mathf.Repeat(
            enrageAngle + enrageRotationPerBurst,
            360f);
    }

    private void SpawnProjectile(Vector2 direction)
    {
        BossProjectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity);

        projectile.Initialize(direction);
        projectiles.Add(projectile);
    }

    public void SummonMinions()
    {
        if (!encounterActive ||
            !(activePhase is Phase2_SummonMinions) ||
            minionWaveSpawned)
        {
            return;
        }

    Debug.Log("[Boss Check] Starting minion spawn.", this);

    int spawned = minionSpawner.SpawnMinions(
    target, projectilePrefab);

    Debug.Log($"[Boss Check] Spawned={spawned}", this);

        minionWaveSpawned = spawned == 3;

        if (!minionWaveSpawned)
        {
            Debug.LogError("Boss minion wave could not start.", this);
            encounterActive = false;
            minionSpawner.ClearMinions();
            bossVisual.enabled = true;
        }
    }

    private void CompleteMinionWave()
    {
        minionSpawner.ClearMinions();
        minionWaveDefeated = true;

        currentHealth = Mathf.Max(
            1, Mathf.FloorToInt(maximumHealth * 0.35f));

        SelectPhase();
    }

    private float GetAttackInterval()
    {
        return activePhase is Phase3_Enrage
            ? Mathf.Max(0.1f, enrageInterval)
            : Mathf.Max(0.1f, attackInterval);
    }

    public void TakeDamage(int amount)
    {
        if (!encounterActive ||
            defeatSignaled ||
            amount <= 0 ||
            activePhase is Phase2_SummonMinions)
        {
            return;
        }

        if (!minionWaveDefeated)
        {
            // The mandatory minion wave cannot be skipped by a large hit.
            int phase2Health =
                Mathf.FloorToInt(maximumHealth * 0.70f);

            currentHealth = Mathf.Max(
                phase2Health, currentHealth - amount);
        }
        else
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
        }

        if (currentHealth == 0)
            Defeat();
        else
            SelectPhase();
    }

    private void Defeat()
    {
        encounterActive = false;
        defeatSignaled = true;

        ClearProjectiles();
        minionSpawner.ClearMinions();

        Debug.Log("Boss defeated.", this);
        OnBossDefeated?.Invoke();
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
        encounterActive = false;
        ClearProjectiles();

        if (minionSpawner != null)
            minionSpawner.ClearMinions();

        if (bossVisual != null)
            bossVisual.enabled = true;
    }

    public BossPhaseInfo GetCurrentPhase()
    {
        string phaseName = encounterActive && activePhase != null
            ? activePhase.PhaseName
            : "None";

        return new BossPhaseInfo(
            phaseName,
            currentHealth,
            maximumHealth,
            defeatSignaled);
    }

    [ContextMenu("Test: Deal 100 Damage")]
    private void TestDamage()
    {
        if (Application.isPlaying)
            TakeDamage(100);
    }

    [ContextMenu("Test: Defeat One Minion")]
    private void TestDefeatOneMinion()
    {
        if (Application.isPlaying &&
            encounterActive &&
            activePhase is Phase2_SummonMinions)
        {
            minionSpawner.DamageOneLivingMinion(int.MaxValue);
        }
    }
}