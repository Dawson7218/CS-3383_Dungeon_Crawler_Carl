using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private GameState gameState;

    [Header("Movement (starting value; stats.moveSpeed is used at runtime)")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    [Header("Room bounds (placeholder)")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);

    [Header("HP")]
    [SerializeField] private int maxHP = 100;

    [Header("Player Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    [Header("Projectile Attack")]
    [SerializeField] private PlayerProjectile projectilePrefab;
    [SerializeField] private float fireInterval = 0.25f;
    [SerializeField] private float projectileSpawnOffset = 0.6f;

    private PlayerStats stats;
    private PlayerState currentState;

    private float dashCooldownTimer = 0f;
    private float fireCooldownTimer = 0f;

    private Vector2 lastDirection = Vector2.right;
    private Vector2 lastAnimationDirection = Vector2.down;
    private KeyCode lastShootKey = KeyCode.None;

    private void Awake()
    {
        stats = new PlayerStats(maxHP, moveSpeed);
        currentState = new PlayerIdleState();

        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        if (playerSpriteRenderer == null)
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (playerAnimator != null)
        {
            // Start facing down. These names must match the Animator parameters.
            playerAnimator.SetFloat("InputX", 0f);
            playerAnimator.SetFloat("InputY", -1f);
            playerAnimator.SetFloat("LastX", 0f);
            playerAnimator.SetFloat("LastY", -1f);
            playerAnimator.SetBool("isWalking", false);
        }

        if (playerSpriteRenderer != null)
            playerSpriteRenderer.flipX = false;

        if (gameState != null)
            gameState.playerHealth = stats.currentHP;
    }

    private void Update()
    {
        if (stats.IsDead())
            return;

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (fireCooldownTimer > 0f)
            fireCooldownTimer -= Time.deltaTime;

        HandleProjectileInput();

        if (currentState is PlayerDashState dash)
        {
            dash.Tick();
            return;
        }

        Vector2 direction = HandleInput();

        if (direction != Vector2.zero)
            lastDirection = direction;

        UpdateMovementAnimation(direction);

        if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0f)
        {
            Vector2 dashDir = direction != Vector2.zero ? direction : lastDirection;


            if (playerAnimator != null)
                playerAnimator.SetBool("isWalking", false);

            ChangeState(new PlayerDashState(this, dashDir, dashDuration));
            return;
        }

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

        if (Input.GetKey(KeyCode.W)) direction.y += 1f;
        if (Input.GetKey(KeyCode.S)) direction.y -= 1f;
        if (Input.GetKey(KeyCode.A)) direction.x -= 1f;
        if (Input.GetKey(KeyCode.D)) direction.x += 1f;

        return direction.normalized;
    }

    private void UpdateMovementAnimation(Vector2 direction)
    {
        if (playerAnimator == null)
            return;

        bool isWalking = direction.sqrMagnitude > 0.001f;

        if (isWalking)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                lastAnimationDirection = new Vector2(Mathf.Sign(direction.x), 0f);
            }
            else
            {
                lastAnimationDirection = new Vector2(0f, Mathf.Sign(direction.y));
            }
        }

        playerAnimator.SetFloat("InputX", lastAnimationDirection.x);
        playerAnimator.SetFloat("InputY", lastAnimationDirection.y);
        playerAnimator.SetFloat("LastX", lastAnimationDirection.x);
        playerAnimator.SetFloat("LastY", lastAnimationDirection.y);
        playerAnimator.SetBool("isWalking", isWalking);

        if (playerSpriteRenderer != null)
            playerSpriteRenderer.flipX = lastAnimationDirection.x > 0f;
    }

    private void HandleProjectileInput()
    {
        Vector2 shootDirection = GetShootDirection();

        if (shootDirection == Vector2.zero)
            return;

        if (fireCooldownTimer > 0f || projectilePrefab == null)
            return;

        int damage = stats.RollAttackDamage(out bool wasCrit);

        Vector2 spawnPosition = (Vector2)transform.position
            + shootDirection * projectileSpawnOffset;

        PlayerProjectile projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity);

        projectile.Initialize(shootDirection, damage);

        fireCooldownTimer = Mathf.Max(0.01f, fireInterval);

        if (wasCrit)
            Debug.Log("Critical hit! Damage: " + damage);
    }

    private Vector2 GetShootDirection()
    {
        // remembers most recent arrow
        if (Input.GetKeyDown(KeyCode.UpArrow))
            lastShootKey = KeyCode.UpArrow;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            lastShootKey = KeyCode.DownArrow;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            lastShootKey = KeyCode.LeftArrow;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            lastShootKey = KeyCode.RightArrow;

        // holding fire
        if (lastShootKey != KeyCode.None && Input.GetKey(lastShootKey))
            return DirectionFromShootKey(lastShootKey);


        if (Input.GetKey(KeyCode.UpArrow))
            lastShootKey = KeyCode.UpArrow;
        else if (Input.GetKey(KeyCode.DownArrow))
            lastShootKey = KeyCode.DownArrow;
        else if (Input.GetKey(KeyCode.LeftArrow))
            lastShootKey = KeyCode.LeftArrow;
        else if (Input.GetKey(KeyCode.RightArrow))
            lastShootKey = KeyCode.RightArrow;
        else
            lastShootKey = KeyCode.None;

        return DirectionFromShootKey(lastShootKey);
    }

    private Vector2 DirectionFromShootKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.UpArrow:
                return Vector2.up;
            case KeyCode.DownArrow:
                return Vector2.down;
            case KeyCode.LeftArrow:
                return Vector2.left;
            case KeyCode.RightArrow:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
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

    public void DashStep(Vector2 direction)
    {
        Vector2 newPosition = (Vector2)transform.position
            + direction.normalized * dashSpeed * Time.deltaTime;

        transform.position = ClampToRoomBounds(newPosition);
    }

    public void EndDash()
    {
        dashCooldownTimer = dashCooldown;
        ChangeState(new PlayerIdleState());
    }

    public Vector2 MovePlayer(Vector2 direction)
    {
        Vector2 newPosition = (Vector2)transform.position
            + direction.normalized * stats.moveSpeed * Time.deltaTime;

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

    public void ApplyDamage(int amount)
    {
        if (stats.IsDead())
            return;

        stats.TakeDamage(amount);

        if (gameState != null)
            gameState.playerHealth = stats.currentHP;

        Debug.Log("Player HP: " + stats.currentHP);

        if (stats.IsDead())
        {
            ChangeState(new PlayerDeadState());
            SceneManager.LoadScene("DeathScene");
        }
    }

    public void TakeDamage(int amount)
    {
        ApplyDamage(amount);
    }

    public bool IsDead => stats.IsDead();
    public int CurrentHP => stats.currentHP;

    public PlayerStats GetStats()
    {
        return stats;
    }
}
