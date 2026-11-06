using UnityEngine;

public class Phase1_RangedBarrage : BossPhase
{
    public override string PhaseName => "Phase 1 - Ranged Barrage";

    public override bool CanHandle(BossController ctx)
    {
        BossPhaseInfo info = ctx.GetCurrentPhase();

        int phase2Threshold =
            Mathf.FloorToInt(info.MaximumHealth * 0.70f);

        return !ctx.MinionWaveDefeated
            && info.CurrentHealth > phase2Threshold;
    }

    public override void ExecutePhase(BossController ctx)
    {
        ctx.FireSpread();
    }
}