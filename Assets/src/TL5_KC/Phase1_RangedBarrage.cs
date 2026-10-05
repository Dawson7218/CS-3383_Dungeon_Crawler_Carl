using UnityEngine;

public class Phase1_RangedBarrage : BossPhase
{
    public override string CanHandle() // same signature​
{
return "breathes fire"; // must be DIFFERENT​
}
}
