using UnityEngine;

public class BossPhase
{
   public virtual string CanHandle() // virtual = overridable​
    {
        return "generic attack"; // RETURN a value​
    }
}
