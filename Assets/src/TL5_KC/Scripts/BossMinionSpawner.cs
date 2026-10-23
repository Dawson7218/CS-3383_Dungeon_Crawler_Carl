using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Minion Prefab")]
    [SerializeField] private EnemyController minionPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int minionsPerSummon = 2;
    [SerializeField] private int maximumLivingMinions = 4;
    [SerializeField] private float spawnRadius = 2f;

    private readonly List<EnemyController> spawnedMinions =
        new List<EnemyController>();

    private int spawnIndex;

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

    public int SpawnMinions(Transform target)
    {
        if (!isActiveAndEnabled)
            return 0;

        if (minionPrefab == null || target == null)
        {
            Debug.LogWarning(
                "Boss summon requires a minion prefab and target.",
                this);
            return 0;
        }

        RemoveDefeatedMinions();

        int availableSlots = Mathf.Max(
            0, maximumLivingMinions - LivingMinionCount);

        int spawnCount = Mathf.Min(
            Mathf.Max(0, minionsPerSummon), availableSlots);

        for (int i = 0; i < spawnCount; i++)
        {
            // Cycle through four positions around the boss.
            float angle = (spawnIndex % 4) * 90f;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f) * Mathf.Max(0f, spawnRadius);

            EnemyController minion = Instantiate(
                minionPrefab,
                transform.position + offset,
                Quaternion.identity);

            minion.SetTarget(target);
            spawnedMinions.Add(minion);

            spawnIndex++;
        }

        return spawnCount;
    }

    private void RemoveDefeatedMinions()
    {
        for (int i = spawnedMinions.Count - 1; i >= 0; i--)
        {
            EnemyController minion = spawnedMinions[i];

            if (minion == null)
            {
                spawnedMinions.RemoveAt(i);
            }
            else if (minion.IsDead)
            {
                Destroy(minion.gameObject);
                spawnedMinions.RemoveAt(i);
            }
        }
    }

    public void ClearMinions()
    {
        foreach (EnemyController minion in spawnedMinions)
        {
            if (minion != null)
                Destroy(minion.gameObject);
        }

        spawnedMinions.Clear();
        spawnIndex = 0;
    }

    private void OnDisable()
    {
        ClearMinions();
    }
}