
using UnityEngine;

public class TestPlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackRange = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackNearestEnemy();
        }
    }

    private void AttackNearestEnemy()
    {
        EnemyController[] enemies =
            FindObjectsByType<EnemyController>(
                FindObjectsSortMode.None
            );

        EnemyController nearestEnemy = null;
        float nearestDistance = attackRange;

        foreach (EnemyController enemy in enemies)
        {
            // Ignore dead enemies.
            if (enemy.IsDead)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy == null)
        {
            Debug.Log("No living enemy in attack range!");
            return;
        }

        nearestEnemy.TakeDamage(damage);

        Debug.Log(
            "Player attacked " + nearestEnemy.name +
            " | Remaining HP: " + nearestEnemy.CurrentHP
        );
    }
}
