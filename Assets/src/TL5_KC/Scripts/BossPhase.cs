public abstract class BossPhase
{
    private BossPhase next;

    public abstract string PhaseName { get; }

    public void SetNext(BossPhase nextPhase)
    {
        next = nextPhase;
    }

    public BossPhase Handle(BossController ctx)
    {
        if (CanHandle(ctx))
            return this;

        return next != null ? next.Handle(ctx) : null;
    }

    public abstract bool CanHandle(BossController ctx);

    public virtual void ExecutePhase(BossController ctx)
    {
    }
}