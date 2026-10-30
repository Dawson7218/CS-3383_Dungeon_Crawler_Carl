public class Phase1_RangedBarrage : BossPhase
{
    public override string PhaseName => "Phase 1 - Ranged Barrage";

    public override bool CanHandle(BossController ctx)
    {
        BossPhaseInfo info = ctx.GetCurrentPhase();

        return info.CurrentHealth > info.MaximumHealth * 0.70f;
    }

    public override void ExecutePhase(BossController ctx)
    {
        ctx.FireSpread();
    }
}