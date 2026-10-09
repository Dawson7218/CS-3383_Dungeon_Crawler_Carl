using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Getty_Testing_Script
{
    // A Test behaves as an ordinary method
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        EconomySaveData candidate = new EconomySaveData();
        CurrencyTransaction baseline = new CurrencyTransaction(candidate);
        CurrencyTransaction actual = new PurchaseTransaction(StatType.Health, 3);
        string a = baseline.Execute(new EconomySaveData());
        string b = actual.Execute(new EconomySaveData());
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    
    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        EconomySaveData candidate = new EconomySaveData();
        CurrencyTransaction baseline = new CurrencyTransaction(candidate); 
        CurrencyTransaction actual = new CurrencyTransaction(candidate);
        //Assert.AreNotEqual(baseline.GetItemId(), actual.GetItemId());
        Assert.AreEqual(baseline.Execute(), actual.Execute());
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
