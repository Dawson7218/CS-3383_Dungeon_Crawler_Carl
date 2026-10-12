
using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private int numberOfEnemies = 3;
    [SerializeField] private float spawnSpacing = 3f;

    void Start()
    {
        SpawnEncounter();
    }

    public void SpawnEncounter()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is not assigned!");
            return;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = transform.position
                + new Vector3(i * spawnSpacing, 0f, 0f);

            GameObject newEnemy = Instantiate(
                enemyPrefab,
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
