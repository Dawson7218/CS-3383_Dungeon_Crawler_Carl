using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DroneProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float lifetime = 3f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isInitialized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.normalized;
        isInitialized = true;

        // Rotate the laser sprite to face its travel direction
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
    }

    private void FixedUpdate()
    {
        if (isInitialized && rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore enemies
        if (other.CompareTag("Enemy") || other.GetComponent<EnemyController>() != null)
            return;

        // Check for the actual PlayerController
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Destroy laser if it hits a wall/obstacle (such as walltop, wallbottom, wallleft, wallright)
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}