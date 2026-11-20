using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private int numberOfEnemies = 3;
    [SerializeField] private float spawnSpacing = 3f;
    [SerializeField] private bool spawnInOrder = true;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        SpawnEncounter();
    }

    public void SpawnEncounter()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Enemy prefabs are not assigned!", this);
            return;
        }

        // Center the group around the spawner's position
        float startOffsetX = -((numberOfEnemies - 1) * spawnSpacing) / 2f;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject prefabToSpawn = spawnInOrder
                ? enemyPrefabs[i % enemyPrefabs.Length]
                : enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            if (prefabToSpawn == null) continue;

            Vector3 spawnPosition = transform.position
                + new Vector3(startOffsetX + (i * spawnSpacing), 0f, 0f);

            GameObject newEnemy = Instantiate(
                prefabToSpawn,
                spawnPosition,
                Quaternion.identity
            );

            EnemyController controller =
                newEnemy.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.SetTarget(player);
            }
        }

        Debug.Log("Encounter spawned: " + numberOfEnemies + " enemies");
    }


}

