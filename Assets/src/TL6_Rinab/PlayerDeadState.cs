using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public override string Update()
    {
        return "dead";
    }
}
