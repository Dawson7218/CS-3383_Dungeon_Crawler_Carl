public class Phase2_SummonMinions : BossPhase
{
    public override string PhaseName => "Phase 2 - Summon Minions";

    public override bool CanHandle(BossController ctx)
    {
        BossPhaseInfo info = ctx.GetCurrentPhase();

        // Handles lower health until we add Phase 3.
        return info.CurrentHealth > 0
            && info.CurrentHealth <= info.MaximumHealth * 0.70f;
    }

    public override void ExecutePhase(BossController ctx)
    {
        ctx.SummonMinions();
    }
}