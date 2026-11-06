
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DroneProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D rb;
    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Initialize(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        initialized = true;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized)
            return;

        // Ignore other enemies.
        if (other.GetComponentInParent<RangedEnemyController>() != null ||
            other.GetComponentInParent<EnemyController>() != null)
            return;

        TestPlayerHealth health =
            other.GetComponentInParent<TestPlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
