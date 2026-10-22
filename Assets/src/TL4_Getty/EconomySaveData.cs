using System;

// Serializable lets Unity's JsonUtility convert this data to and from JSON.
[Serializable]
public class EconomySaveData
{
    // These public fields are the values written to economy.json.
    // Levels count purchases; the actual stat bonuses are calculated by EconomyManager.
    public int points;
    public int healthLevel;
    public int attackLevel;
    public int defenseLevel;

    public EconomySaveData Clone()
    {
        // Make a separate candidate for a transaction to edit before it is saved.
        // MemberwiseClone is sufficient because every field here is an int value.
        return (EconomySaveData)MemberwiseClone();
    }

    public bool IsValid()
    {
        // Negative currency or upgrade levels are not allowed in a save or transaction.
        return points >= 0 && healthLevel >= 0 && attackLevel >= 0 && defenseLevel >= 0;
    }
}
