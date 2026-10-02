using UnityEngine;

public class HudElement
{
    public GameState _gameState;

    public virtual string Refresh(GameState gameState)
    {
        return "BaseRefresh";
    }
}
