
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int maxHealth = 100;

    private Enemy enemy;
    private Rigidbody2D rb;
    private EnemyState currentState;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        enemy = new Enemy(1, maxHealth, new IdleState());
        currentState = new IdleState();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (enemy.IsDefeated())
        {
            ChangeState(new DeadState());
            return;
        }

        if (player == null)
        {
            ChangeState(new IdleState());
            return;
        }

        float distance = Vector2.Distance(
            rb.position,
            player.position
        );

        if (distance <= attackRange)
        {
            ChangeState(new AttackState());
        }
        else if (distance <= detectionRange)
        {
            ChangeState(new ChaseState());

            Vector2 direction =
                ((Vector2)player.position - rb.position).normalized;

            Vector2 newPosition =
                rb.position +
                direction * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);
        }
        else
        {
            ChangeState(new IdleState());
        }
    }


private void ChangeState(EnemyState newState)
{
    if (currentState.GetType() == newState.GetType())
        return;

    currentState = newState;
    enemy.ChangeState(newState);

    if (animator != null)
    {
        if (newState is IdleState)
        {
            animator.Play("Enemy_Idle");
        }
        else if (newState is ChaseState)
        {
            animator.Play("Enemy_Run");
        }
        else if (newState is AttackState)
        {
            animator.Play("Enemy_Attack");
        }
        else if (newState is DeadState)
        {
            animator.Play("Enemy_Die");
        }
    }

    Debug.Log(enemy.Update());
}


    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void TakeDamage(int damage)
    {
        enemy.TakeDamage(damage);

        if (enemy.IsDefeated())
        {
            ChangeState(new DeadState());
        }
    }
}
