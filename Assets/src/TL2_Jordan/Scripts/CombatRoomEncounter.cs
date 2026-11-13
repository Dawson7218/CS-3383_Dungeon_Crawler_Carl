using System.Collections.Generic;
using UnityEngine;

public class CombatRoomEncounter : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Range(1, 9)] private int enemyCount = 3;
    [Tooltip("Multiplier for the enemy prefab's original scale. Use (1, 1, 1) to preserve its intended size.")]
    [SerializeField] private Vector3 enemyScale = Vector3.one;
    [SerializeField, Min(0f)] private float minimumPlayerDistance = 3f;
    [SerializeField, Min(0f)] private float spawnClearance = 0.5f;

    public bool HasSpawned { get; private set; }

    public void TrySpawn(Transform player)
    {
        if (HasSpawned || player == null)
            return;

        if (enemyPrefab == null || enemyPrefab.GetComponent<EnemyController>() == null)
        {
            Debug.LogWarning("Combat room needs a prefab with an EnemyController.", this);
            return;
        }

        // Stay on the clear central arena, away from walls and decorative pillars.
        var positions = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
                positions.Add(transform.position + new Vector3(x * 3f, y * 2f, 0f));
        }

        // Shuffle once so every room need not use the same starting arrangement.
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int other = Random.Range(0, i + 1);
            Vector3 temporary = positions[i];
            positions[i] = positions[other];
            positions[other] = temporary;
        }

        HasSpawned = true;
        int spawned = 0;
        int requested = Mathf.Clamp(enemyCount, 1, positions.Count);
        foreach (Vector3 position in positions)
        {
            if (Vector2.Distance(position, player.position) < minimumPlayerDistance ||
                Physics2D.OverlapCircle(position, spawnClearance) != null)
                continue;

            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
            enemy.name = "RoomEnemy_" + (spawned + 1);
            enemy.transform.localScale = Vector3.Scale(enemy.transform.localScale, enemyScale);
            ConfigureEnemyFootprint(enemy);
            PreventPlayerShoving(enemy, player);
            enemy.GetComponent<EnemyController>().SetTarget(player);

            if (++spawned == requested)
                break;
        }

        Debug.Log("Combat encounter in " + name + ": spawned " + spawned + " enemies.", this);
        if (spawned < requested)
            Debug.LogWarning("Not enough clear positions for all requested enemies in " + name + ".", this);
    }

    private static void PreventPlayerShoving(GameObject enemy, Transform player)
    {
        // Melee damage uses EnemyController's distance check, not physical contact.
        // Ignore only solid enemy/player pairs; retain walls and projectile triggers.
        foreach (Collider2D enemyCollider in enemy.GetComponentsInChildren<Collider2D>())
        {
            if (enemyCollider.isTrigger)
                continue;

            foreach (Collider2D playerCollider in player.GetComponentsInChildren<Collider2D>())
            {
                if (!playerCollider.isTrigger)
                    Physics2D.IgnoreCollision(enemyCollider, playerCollider);
            }
        }
    }

    private static void ConfigureEnemyFootprint(GameObject enemy)
    {
        BoxCollider2D bodyCollider = enemy.GetComponent<BoxCollider2D>();
        SpriteRenderer renderer = enemy.GetComponent<SpriteRenderer>();
        if (bodyCollider == null || renderer == null || renderer.sprite == null)
            return;

        // The source prefab uses a unit box even though its sprite is much narrower.
        // Use a stable foot-level footprint for top-down collision, in unscaled units.
        Bounds spriteBounds = renderer.sprite.bounds;
        bodyCollider.size = new Vector2(spriteBounds.size.x * 0.8f, spriteBounds.size.y * 0.35f);
        bodyCollider.offset = (Vector2)spriteBounds.center +
            Vector2.down * spriteBounds.size.y * 0.3f;
    }
}
