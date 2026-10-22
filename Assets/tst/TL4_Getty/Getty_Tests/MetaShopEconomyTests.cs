using NUnit.Framework;

// These tests use temporary data objects, so they never spend the player's saved points.
public class MetaShopEconomyTests
{
    // NUnit runs this same test once for each TestCase stat.
    [TestCase(StatType.Health)]
    [TestCase(StatType.Defense)]
    [TestCase(StatType.Attack)]
    public void Purchase_DeductsPointsAndLevelsOnlySelectedStat(StatType stat)
    {
        // Arrange a 100-point balance, then execute one 25-point upgrade.
        EconomySaveData candidate = new EconomySaveData { points = 100 };
        PurchaseTransaction transaction = new PurchaseTransaction(stat, 25);
        transaction.Execute(candidate);

        // Check both the cost and that the other two stats did not change.
        Assert.IsTrue(transaction.Succeeded);
        Assert.AreEqual(75, candidate.points);
        Assert.AreEqual(stat == StatType.Health ? 1 : 0, candidate.healthLevel);
        Assert.AreEqual(stat == StatType.Defense ? 1 : 0, candidate.defenseLevel);
        Assert.AreEqual(stat == StatType.Attack ? 1 : 0, candidate.attackLevel);
    }

    [Test]
    public void Purchase_ExactBalance_SpendsToZero()
    {
        // Spending the entire balance is a valid purchase, not an insufficient-funds error.
        EconomySaveData candidate = new EconomySaveData { points = 25 };
        PurchaseTransaction transaction = new PurchaseTransaction(StatType.Health, 25);
        transaction.Execute(candidate);
        Assert.IsTrue(transaction.Succeeded);
        Assert.AreEqual(0, candidate.points);
    }

    // Cover insufficient funds, a negative price, and an unsupported enum value.
    [TestCase(24, 25, StatType.Health)]
    [TestCase(100, -1, StatType.Health)]
    [TestCase(100, 25, (StatType)99)]
    public void RejectedPurchase_DoesNotChangeState(int balance, int cost, StatType stat)
    {
        // A rejected transaction must leave both currency and all three levels unchanged.
        EconomySaveData candidate = new EconomySaveData { points = balance };
        PurchaseTransaction transaction = new PurchaseTransaction(stat, cost);
        transaction.Execute(candidate);
        Assert.IsFalse(transaction.Succeeded);
        Assert.AreEqual(balance, candidate.points);
        Assert.AreEqual(0, candidate.healthLevel + candidate.defenseLevel + candidate.attackLevel);
    }

    [Test]
    public void Purchase_LevelOverflow_DoesNotSpendPoints()
    {
        // int.MaxValue cannot be increased safely, even when the player can pay.
        EconomySaveData candidate = new EconomySaveData { points = 100, healthLevel = int.MaxValue };
        PurchaseTransaction transaction = new PurchaseTransaction(StatType.Health, 25);
        transaction.Execute(candidate);
        Assert.IsFalse(transaction.Succeeded);
        Assert.AreEqual(100, candidate.points);
        Assert.AreEqual(int.MaxValue, candidate.healthLevel);
    }

    [Test]
    public void Payout_AddsPointsWithoutChangingLevels()
    {
        // A run payout changes the wallet but must preserve previously purchased upgrades.
        EconomySaveData candidate = new EconomySaveData { points = 10, defenseLevel = 2 };
        RunEndPayoutTransaction transaction = new RunEndPayoutTransaction(50);
        transaction.Execute(candidate);
        Assert.IsTrue(transaction.Succeeded);
        Assert.AreEqual(60, candidate.points);
        Assert.AreEqual(2, candidate.defenseLevel);
    }

    // Check a negative payout and a payout that would overflow the balance.
    [TestCase(-1, 10)]
    [TestCase(1, int.MaxValue)]
    public void InvalidPayout_DoesNotChangeBalance(int payout, int balance)
    {
        EconomySaveData candidate = new EconomySaveData { points = balance };
        RunEndPayoutTransaction transaction = new RunEndPayoutTransaction(payout);
        transaction.Execute(candidate);
        Assert.IsFalse(transaction.Succeeded);
        Assert.AreEqual(balance, candidate.points);
    }

    [Test]
    public void CandidateTransaction_DoesNotMutateOriginalSave()
    {
        // Editing a cloned candidate must not affect the original accepted save object.
        EconomySaveData original = new EconomySaveData { points = 100 };
        EconomySaveData candidate = original.Clone();
        new PurchaseTransaction(StatType.Attack, 25).Execute(candidate);
        Assert.AreEqual(100, original.points);
        Assert.AreEqual(0, original.attackLevel);
        Assert.AreEqual(75, candidate.points);
        Assert.AreEqual(1, candidate.attackLevel);
    }
}
