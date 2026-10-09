using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    private EconomySaveData state;

    // Per-stat costs and bonuses
    // TODO: Define cost and bonus configurations.

    private void Awake()
    {
        // TODO: Implement Singleton.
        // TODO: Load saved economy state.
    }

    public static EconomyManager GetInstance()
    {
        return Instance;
    }

    public int GetBalance()
    {
        return state.points;
        throw new System.NotImplementedException();
    }

    public int GetLevel(StatType stat)
    {
        // TODO: Return level for selected stat.
        throw new System.NotImplementedException();
    }

    public int GetNextCost(StatType stat)
    {
        // TODO: Calculate cost of next upgrade.
        throw new System.NotImplementedException();
    }

    public bool TryPurchase(StatType stat)
    {
        // TODO: Validate purchase.
        // TODO: Create PurchaseTransaction.
        // TODO: Execute against candidate state.
        // TODO: Save candidate before committing.
        throw new System.NotImplementedException();
    }

    public void CreditRunEnd(int pointsEarned)
    {
        // TODO: Validate payout.
        // TODO: Create RunEndPayoutTransaction.
        // TODO: Execute against candidate state.
        // TODO: Save candidate before committing.
        throw new System.NotImplementedException();
    }

    private void Load()
    {
        // TODO: Load saved EconomySaveData.
    }

    private bool Save(EconomySaveData candidate)
    {
        // TODO: Persist candidate state.
        throw new System.NotImplementedException();
    }
}