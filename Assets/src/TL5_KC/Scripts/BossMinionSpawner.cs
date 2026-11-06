using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Minion Prefab")]
    [SerializeField] private EnemyController minionPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 2f;

    private const int WaveSize = 3;

    private readonly List<EnemyController> spawnedMinions =
        new List<EnemyController>();

    public bool IsReady =>
        isActiveAndEnabled
        && minionPrefab != null
        && minionPrefab.GetComponent<BossMinionAttack>() != null;

    public int LivingMinionCount
    {
        get
        {
            int count = 0;

            foreach (EnemyController minion in spawnedMinions)
            {
                if (minion != null && !minion.IsDead)
                    count++;
            }

            return count;
        }
    }

    public int SpawnMinions(
        Transform target,
        BossProjectile projectilePrefab)
    {
        if (!IsReady || target == null || projectilePrefab == null)
        {
            Debug.LogError(
                "Assign a boss minion prefab with BossMinionAttack, " +
                "a target, and a projectile prefab.",
                this);

            return 0;
        }

        ClearMinions();

        for (int i = 0; i < WaveSize; i++)
        {
            float angle = i * 120f;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f) * spawnRadius;

            EnemyController minion = Instantiate(
                minionPrefab,
                transform.position + offset,
                Quaternion.identity);

            BossMinionAttack attack =
                minion.GetComponent<BossMinionAttack>();

            attack.Initialize(
                target,
                projectilePrefab,
                angle,
                1f + i * 0.4f);

            spawnedMinions.Add(minion);
        }

        return WaveSize;
    }

    public void ClearMinions()
    {
        foreach (EnemyController minion in spawnedMinions)
        {
            if (minion != null)
                Destroy(minion.gameObject);
        }

        spawnedMinions.Clear();
    }

    // Used by the boss's temporary test command.
    public void DamageOneLivingMinion(int amount)
    {
        foreach (EnemyController minion in spawnedMinions)
        {
            if (minion != null && !minion.IsDead)
            {
                minion.TakeDamage(amount);
                return;
            }
        }
    }

    private void OnDisable()
    {
        ClearMinions();
    }
}