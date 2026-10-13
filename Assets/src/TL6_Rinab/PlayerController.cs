using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Room bounds (placeholder)")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);

    [Header("HP")]
    [SerializeField] private int maxHP = 100;

    private PlayerStats stats;
    private PlayerState currentState;

    void Awake()
    {
        stats = new PlayerStats(maxHP);
        currentState = new PlayerIdleState();
    }

    void Update()
    {
        Vector2 direction = HandleInput();

        if (direction != Vector2.zero)
        {
            MovePlayer(direction);
            ChangeState(new PlayerMovingState());
        }
        else
        {
            ChangeState(new PlayerIdleState());
        }
    }

    private Vector2 HandleInput()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) direction.y += 1;
        if (Input.GetKey(KeyCode.S)) direction.y -= 1;
        if (Input.GetKey(KeyCode.A)) direction.x -= 1;
        if (Input.GetKey(KeyCode.D)) direction.x += 1;

        return direction.normalized;
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState.GetType() == newState.GetType())
            return;

        currentState = newState;
        Debug.Log(currentState.Update());
    }

    public string GetCurrentStateDescription()
    {
        return currentState.Update();
    }

    public Vector2 MovePlayer(Vector2 direction)
    {
        Vector2 newPosition = (Vector2)transform.position
            + direction.normalized * moveSpeed * Time.deltaTime;

        newPosition = ClampToRoomBounds(newPosition);
        transform.position = newPosition;
        return newPosition;
    }

    private Vector2 ClampToRoomBounds(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        return pos;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }


    public void TakeDamage(int amount)
    {
        stats.TakeDamage(amount);

        Debug.Log("Player HP: " + stats.currentHP);

        if (stats.IsDead())
        {
            Debug.Log("Player died");
        }
    }
    public bool IsDead => stats.IsDead();
    public int CurrentHP => stats.currentHP;

    public PlayerStats GetStats()
    {
        return stats;
    }
}