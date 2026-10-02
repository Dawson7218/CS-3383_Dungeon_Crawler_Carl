using UnityEngine;

public class RunEndPayoutTransaction : CurrencyTransaction
{
    int pointsEarned;
    public override string Execute() //fix this later, only no (EconomySaveData candidate) because of 
    {
        return "This is an override string";
    }
}
