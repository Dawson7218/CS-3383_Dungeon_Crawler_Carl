using UnityEngine;

public class PurchaseTransaction : CurrencyTransaction
{
    int cost;
    StatType state;

    public override string Execute()
    {
        return "other override string";
    }
}
