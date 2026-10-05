using UnityEngine;

public class HealthBarElement : HudElement
{
    public override string Refresh(GameState gameState)
    {
        return "HealthBar";
    }
}
