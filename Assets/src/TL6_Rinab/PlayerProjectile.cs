using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;

    private Rigidbody2D rb;
    private Collider2D projectileCollider;

    private Transform projectileOwner;

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

    public void Initialize(
        Vector2 shootDirection,
        int damageAmount,
        Transform owner)
    {
        if (shootDirection.sqrMagnitude < 0.001f)
        {
            Destroy(gameObject);
            return;
        }

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

        damage = Mathf.Max(0, damageAmount);
        lifeRemaining = lifetime;

        projectileOwner = owner;
        initialized = true;

        if (projectileOwner != null)
        {
            Collider2D[] ownerColliders =
                projectileOwner.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D ownerCollider in ownerColliders)
            {
                if (ownerCollider != null &&
                    ownerCollider != projectileCollider)
                {
                    Physics2D.IgnoreCollision(
                        projectileCollider,
                        ownerCollider,
                        true
                    );
                }
            }
        }

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
        if (!initialized || hasHit || other == null)
            return;

        if (projectileOwner != null &&
            (other.transform == projectileOwner ||
             other.transform.IsChildOf(projectileOwner)))
        {
            return;
        }

        BossController boss =
            other.GetComponentInParent<BossController>();

        if (boss != null)
        {
            boss.TakeDamage(damage);

            Debug.Log("Projectile damaged boss: " + damage);

            DestroyProjectile();
            return;
        }

        EnemyController enemy =
            other.GetComponentInParent<EnemyController>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Debug.Log("Projectile damaged enemy: " + damage);

            DestroyProjectile();
            return;
        }

        if (!other.isTrigger)
        {
            Debug.Log(
                "Projectile hit obstacle: " + other.name
            );

            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        if (hasHit)
            return;

        hasHit = true;

        rb.linearVelocity = Vector2.zero;

        if (projectileCollider != null)
            projectileCollider.enabled = false;

        Destroy(gameObject);
    }
}