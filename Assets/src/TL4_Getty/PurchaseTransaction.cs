public class PurchaseTransaction : CurrencyTransaction
{
    private StatType stat;
    private int cost;

    public PurchaseTransaction(StatType stat, int cost)
    {
        this.stat = stat;
        this.cost = cost;
    }

    public override void Execute(EconomySaveData candidate)
    {
        // TODO: Deduct cost from candidate.points.
        // TODO: Increment corresponding stat level.
    }
}