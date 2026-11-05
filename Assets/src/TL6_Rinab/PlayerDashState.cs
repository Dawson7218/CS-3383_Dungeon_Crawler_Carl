using UnityEngine;

public class PlayerDashState : PlayerState
{
    private PlayerController ctx;
    private float dashTimer;
    private Vector2 dashDirection;

    public PlayerDashState(PlayerController ctx, Vector2 direction, float duration)
    {
        this.ctx = ctx;
        dashDirection = direction.normalized;
        dashTimer = duration;
    }

    public override string Update()
    {
        return "dashing";
    }

    public void Tick()
    {
        dashTimer -= Time.deltaTime;
        ctx.DashStep(dashDirection);

        if (dashTimer <= 0f)
        {
            ctx.EndDash();
        }
    }
}