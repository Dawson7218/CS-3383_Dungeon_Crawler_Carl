using UnityEngine;

public class Phase2_SummonMinions : BossPhase
{
    public override string PhaseName => "Phase 2 - Summon Minions";

    public override bool CanHandle(BossController ctx)
    {
        BossPhaseInfo info = ctx.GetCurrentPhase();

        int phase2Threshold =
            Mathf.FloorToInt(info.MaximumHealth * 0.70f);

        return info.CurrentHealth > 0
            && info.CurrentHealth <= phase2Threshold
            && !ctx.MinionWaveDefeated;
    }

    public override void ExecutePhase(BossController ctx)
    {
        ctx.SummonMinions();
    }
}