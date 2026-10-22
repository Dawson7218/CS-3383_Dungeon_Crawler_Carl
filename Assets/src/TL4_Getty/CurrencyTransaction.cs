// Common parent for operations that change points or upgrade levels.
public class CurrencyTransaction
{
    // Callers can read the result; this class and its subclasses can set it.
    public bool Succeeded { get; protected set; }

    // virtual lets PurchaseTransaction and RunEndPayoutTransaction supply their own logic.
    // A CurrencyTransaction reference still calls the actual subclass's override.
    // candidate is the copy prepared by EconomyManager, not the accepted save state.
    public virtual string Execute(EconomySaveData candidate)
    {
        // The base implementation performs no operation and reports no success.
        Succeeded = false;
        return "No currency transaction selected.";
    }
}
