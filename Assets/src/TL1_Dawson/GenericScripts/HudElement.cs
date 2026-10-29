using UnityEngine;

public class HudElement
{
    public virtual bool Refresh(GameState gameState)
    {
        return true;
    }
}
