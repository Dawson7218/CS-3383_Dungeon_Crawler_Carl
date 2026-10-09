public class RunEndPayoutTransaction : CurrencyTransaction
{
    private int pointsEarned;

    public RunEndPayoutTransaction(int pointsEarned)
    {
        this.pointsEarned = pointsEarned;
    }

    public override string Execute(EconomySaveData candidate)
    {
        return "string1";
    }
}