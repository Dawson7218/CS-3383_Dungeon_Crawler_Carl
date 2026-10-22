using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BossProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private bool movementEnabled = true;

    [Header("Damage")]
    [SerializeField] private int damage = 10;
    [SerializeField] private bool damageEnabled = true;

    [Header("Lifetime")]
    [SerializeField] private bool autoExpire = true;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D body;
    private float elapsedTime;
    private bool hasHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 0f;
        body.linearDamping = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        hasHit = false;
    }

    private void FixedUpdate()
    {
        body.linearVelocity = movementEnabled && !hasHit
            ? direction.normalized * speed
            : Vector2.zero;
    }

    private void Update()
    {
        if (!autoExpire)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
            Destroy(gameObject);
    }

    // The boss calls this after spawning a projectile.
    public void Initialize(Vector2 shotDirection)
    {
        direction = shotDirection.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!damageEnabled || hasHit)
            return;

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null || player.IsDead)
            return;

        hasHit = true;
        body.linearVelocity = Vector2.zero;

        player.TakeDamage(damage);
        Destroy(gameObject);
    }
}