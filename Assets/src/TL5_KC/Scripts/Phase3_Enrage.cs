public class Phase3_Enrage : BossPhase
{
    public override string PhaseName => "Phase 3 - Enrage";

    public override bool CanHandle(BossController ctx)
    {
        return ctx.MinionWaveDefeated
            && ctx.GetCurrentPhase().CurrentHealth > 0;
    }

    public override void ExecutePhase(BossController ctx)
    {
        ctx.FireRadialBurst();
    }
}