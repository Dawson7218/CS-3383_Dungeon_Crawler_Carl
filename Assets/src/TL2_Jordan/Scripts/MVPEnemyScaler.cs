using System.Collections;
using UnityEngine;

public class MVPEnemyScaler : MonoBehaviour
{
    [SerializeField]
    private float enemyScale = 0.25f;

    private IEnumerator Start()
    {
        // Wait until all Start methods have had a chance to run.
        yield return null;

        EnemyController enemy = FindAnyObjectByType<EnemyController>();

        if (enemy != null)
        {
            enemy.transform.localScale = new Vector3(
                -enemyScale,
                enemyScale,
                enemyScale
            );

            Debug.Log("MVP enemy resized and flipped!");
        }
        else
        {
            Debug.LogWarning("No enemy found to resize.");
        }
    }
}
