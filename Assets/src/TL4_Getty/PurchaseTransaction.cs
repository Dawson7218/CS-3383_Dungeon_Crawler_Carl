public class PurchaseTransaction : CurrencyTransaction
{
    // Remember what this purchase buys and costs. readonly keeps these fixed after construction.
    private readonly StatType stat;
    private readonly int cost;

    public PurchaseTransaction(StatType stat, int cost)
    {
        // "this.stat" is the field; "stat" is the argument passed to the constructor.
        this.stat = stat;
        this.cost = cost;
    }

    public override string Execute(EconomySaveData candidate)
    {
        // Reset the result in case the same transaction object is executed again.
        Succeeded = false;
        // Validate everything before changing points or levels.
        if (candidate == null || !candidate.IsValid() || cost < 0)
            return "Invalid purchase.";
        if (candidate.points < cost)
            return "Not enough points.";

        // Increase only the requested stat by one level.
        // Each int.MaxValue check prevents an integer from overflowing into a negative value.
        switch (stat)
        {
            case StatType.Health:
                if (candidate.healthLevel == int.MaxValue) return "Health is at its limit.";
                candidate.healthLevel++;
                break;
            case StatType.Defense:
                if (candidate.defenseLevel == int.MaxValue) return "Defense is at its limit.";
                candidate.defenseLevel++;
                break;
            case StatType.Attack:
                if (candidate.attackLevel == int.MaxValue) return "Attack is at its limit.";
                candidate.attackLevel++;
                break;
            default:
                // Reject an enum value that does not name one of the supported stats.
                return "Unknown stat.";
        }

        // Charge the candidate only after the selected level was successfully increased.
        // EconomyManager still has to save this candidate before accepting the purchase.
        candidate.points -= cost;
        Succeeded = true;
        return stat + " upgraded.";
    }
}
