public class RunEndPayoutTransaction : CurrencyTransaction
{
    private int pointsEarned;

    public RunEndPayoutTransaction(int pointsEarned)
    {
        this.pointsEarned = pointsEarned;
    }

    public override void Execute(EconomySaveData candidate)
    {
        // TODO: Add pointsEarned to candidate.points.
    }
}