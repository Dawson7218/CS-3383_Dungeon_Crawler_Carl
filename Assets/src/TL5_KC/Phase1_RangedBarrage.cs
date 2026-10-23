public class Phase1_RangedBarrage : BossPhase
{
    public override string PhaseName => "Phase 1 - Ranged Barrage";

    public override bool CanHandle(BossController ctx)
    {
        // Temporary: Phase 1 handles all living health values.
        // We will narrow this range when Phases 2 and 3 are added.
        return ctx.GetCurrentPhase().CurrentHealth > 0;
    }

    public override void ExecutePhase(BossController ctx)
    {
        ctx.FireSpread();
    }
}