// Adds the final points earned by a completed run without changing upgrade levels.
public class RunEndPayoutTransaction : CurrencyTransaction
{
    // Store the payout supplied by future run/death handling code.
    private readonly int pointsEarned;

    public RunEndPayoutTransaction(int pointsEarned)
    {
        this.pointsEarned = pointsEarned;
    }

    public override string Execute(EconomySaveData candidate)
    {
        // Reject missing/invalid save data and negative payouts before editing anything.
        Succeeded = false;
        if (candidate == null || !candidate.IsValid() || pointsEarned < 0)
            return "Invalid run payout.";
        // Add using long first so the overflow check itself cannot overflow an int.
        if ((long)candidate.points + pointsEarned > int.MaxValue)
            return "Point balance would exceed its limit.";

        // Update only the candidate. EconomyManager is responsible for saving it.
        candidate.points += pointsEarned;
        Succeeded = true;
        return pointsEarned + " points earned.";
    }
}
