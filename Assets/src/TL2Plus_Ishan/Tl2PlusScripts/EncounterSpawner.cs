using System.Collections.Generic;
using UnityEngine;

// Spawns the enemies for a room. Kept separate from Room so the room
// doesn't need to know anything about how enemies are created.
public class EncounterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class TierConfig
    {
        public int tier = 1;
        public GameObject[] enemyPrefabs;
        public int enemyCount = 3;
    }

    [Header("Difficulty Tiers")]
    [SerializeField] private TierConfig[] tiers;

    [Header("Old settings (only used if no tiers are set up)")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int numberOfEnemies = 3;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnSpacing = 3f;
    [SerializeField] private bool spawnInOrder = true;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int defaultTier = 1;

    private readonly List<EnemyController> activeEnemies =
        new List<EnemyController>();

    // Enemies from this spawner that are still alive.
    public int ActiveEnemyCount
    {
        get
        {
            activeEnemies.RemoveAll(e => e == null || e.IsDead);
            return activeEnemies.Count;
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        if (spawnOnStart)
            SpawnEncounter(transform, defaultTier);
    }

    // Still here so anything calling the old version keeps working.
    public void SpawnEncounter()
    {
        SpawnEncounter(transform, defaultTier);
    }

    public void SpawnEncounter(Transform room, int difficultyTier)
    {
        if (room == null)
        {
            Debug.LogWarning("SpawnEncounter: room is null, nothing spawned.", this);
            return;
        }

        if (!TryGetConfig(difficultyTier, out GameObject[] prefabs, out int count))
        {
            Debug.LogWarning(
                "No enemies set up for tier " + difficultyTier, this);
            return;
        }

        // Center the group around the room position
        float startOffsetX = -((count - 1) * spawnSpacing) / 2f;
        int spawned = 0;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = spawnInOrder
                ? prefabs[i % prefabs.Length]
                : prefabs[Random.Range(0, prefabs.Length)];

            // Skip empty slots, the rest can still spawn.
            if (prefab == null)
                continue;

            Vector3 position = room.position
                + new Vector3(startOffsetX + (i * spawnSpacing), 0f, 0f);

            GameObject instance =
                Instantiate(prefab, position, Quaternion.identity);

            EnemyController enemy = instance.GetComponent<EnemyController>();

            if (enemy == null)
            {
                Debug.LogWarning(
                    prefab.name + " has no EnemyController, removing it.", this);
                Destroy(instance);
                continue;
            }

            enemy.SetTarget(player);
            activeEnemies.Add(enemy);
            spawned++;
        }

        Debug.Log("Encounter spawned: " + spawned + " enemies (tier "
            + difficultyTier + ")");
    }

    private bool TryGetConfig(int tier, out GameObject[] prefabs, out int count)
    {
        if (tiers != null && tiers.Length > 0)
        {
            foreach (TierConfig config in tiers)
            {
                if (config != null && config.tier == tier &&
                    config.enemyPrefabs != null &&
                    config.enemyPrefabs.Length > 0)
                {
                    prefabs = config.enemyPrefabs;
                    count = config.enemyCount;
                    return true;
                }
            }

            prefabs = null;
            count = 0;
            return false;
        }

        // No tiers set up yet, fall back to the old list.
        prefabs = enemyPrefabs;
        count = numberOfEnemies;
        return enemyPrefabs != null && enemyPrefabs.Length > 0;
    }
}
