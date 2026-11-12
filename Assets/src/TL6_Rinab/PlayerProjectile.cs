
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private LayerMask obstacleLayers;

    private Rigidbody2D rb;
    private Collider2D projectileCollider;

    private Vector2 direction;
    private int damage;

    private float lifeRemaining;

    private bool initialized;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        projectileCollider.isTrigger = true;
    }

    public void Initialize(Vector2 shootDirection, int damageAmount)
    {
        if (Mathf.Abs(shootDirection.x) >=
            Mathf.Abs(shootDirection.y))
        {
            direction = new Vector2(
                Mathf.Sign(shootDirection.x), 0f);
        }
        else
        {
            direction = new Vector2(
                0f, Mathf.Sign(shootDirection.y));
        }

        if (shootDirection.sqrMagnitude < 0.001f)
        {
            Destroy(gameObject);
            return;
        }

        damage = Mathf.Max(0, damageAmount);

        lifeRemaining = lifetime;
        initialized = true;

        rb.linearVelocity = direction * speed;
    }

    private void Update()
    {
        if (!initialized || hasHit)
            return;

        lifeRemaining -= Time.deltaTime;

        if (lifeRemaining <= 0f)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized || hasHit)
            return;

        BossController boss =
            other.GetComponentInParent<BossController>();

        if (boss != null)
        {
            boss.TakeDamage(damage);
            DestroyProjectile();
            return;
        }

        if (IsInLayerMask(other.transform, damageableLayers))
        {
            other.SendMessageUpwards(
                "TakeDamage",
                damage,
                SendMessageOptions.DontRequireReceiver
            );

            DestroyProjectile();
            return;
        }

        if (IsInLayerMask(other.transform, obstacleLayers))
        {
            DestroyProjectile();
        }
    }

    private bool IsInLayerMask(
        Transform current,
        LayerMask mask)
    {
        while (current != null)
        {
            if ((mask.value &
                (1 << current.gameObject.layer)) != 0)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private void DestroyProjectile()
    {
        if (hasHit)
            return;

        hasHit = true;

        rb.linearVelocity = Vector2.zero;

        Destroy(gameObject);
    }
}
