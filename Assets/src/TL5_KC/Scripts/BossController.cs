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

    [Header("Ranged Barrage")]
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float spreadAngle = 15f;

    [Header("Minion Summoning")]
    [SerializeField] private BossMinionSpawner minionSpawner;
    [SerializeField] private float summonInterval = 4f;

    private int currentHealth;
    private float attackTimer;
    private bool encounterActive;
    private bool defeatSignaled;
    private BossPhase chainHead;
    private BossPhase activePhase;

    private readonly List<BossProjectile> projectiles =
        new List<BossProjectile>();

    public event Action OnBossDefeated;

    private void Start()
    {
        if (startAutomatically)
            StartEncounter();
    }

    public void StartEncounter()
    {
        if (encounterActive || defeatSignaled)
            return;

        if (maximumHealth <= 0 ||
            projectilePrefab == null ||
            firePoint == null ||
            attackInterval <= 0f)
        {
            Debug.LogError(
                "Boss setup invalid: check health, projectile, " +
                "fire point, and attack interval.", this);
            return;
        }

        currentHealth = maximumHealth;

            chainHead = new Phase1_RangedBarrage();
            chainHead.SetNext(new Phase2_SummonMinions());
            activePhase = chainHead.Handle(this);

            attackTimer = attackInterval;
            encounterActive = true;

        Debug.Log("Boss encounter started: " + activePhase.PhaseName, this);
    }

    private void Update()
    {
        if (!encounterActive)
        return;

        BossPhase selectedPhase = chainHead.Handle(this);

        if (selectedPhase == null)
    {
        Debug.LogError("No boss phase can handle the current health.", this);
        encounterActive = false;
        ClearProjectiles();
        return;
    }

    if (selectedPhase != activePhase)
    {
        activePhase = selectedPhase;
        attackTimer = attackInterval;

        Debug.Log("Boss phase changed: " + activePhase.PhaseName, this);
    }

    attackTimer -= Time.deltaTime;

    if (attackTimer > 0f)
        return;

    activePhase.ExecutePhase(this);
    attackTimer = GetAttackInterval();
    }

    public void FireSpread()
    {
        // Without a target, shoot downward for standalone testing.
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
            Vector2 shotDirection =
                Quaternion.Euler(0f, 0f, i * spreadAngle)
                * (Vector3)aimDirection;

            BossProjectile projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity);

            projectile.Initialize(shotDirection);
            projectiles.Add(projectile);
        }
    }

    public void SummonMinions()
{
    if (minionSpawner == null || target == null)
    {
        Debug.LogWarning("Assign the boss minion spawner and target.", this);
        return;
    }

    minionSpawner.SpawnMinions(target);
}

private float GetAttackInterval()
{
    return activePhase is Phase2_SummonMinions
        ? Mathf.Max(0.1f, summonInterval)
        : Mathf.Max(0.1f, attackInterval);
}
    public void TakeDamage(int amount)
    {
        if (!encounterActive || defeatSignaled || amount <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (currentHealth == 0)
            Defeat();
    }

    private void Defeat()
    {
        encounterActive = false;
        defeatSignaled = true;

        ClearProjectiles();

        if (minionSpawner != null)
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
}