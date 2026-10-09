using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Getty_Testing_Script
{
    // A Test behaves as an ordinary method
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        CurrencyTransaction baseline = new CurrencyTransaction();
        CurrencyTransaction actual = new PurchaseTransaction(StatType.Health, 3);
        string a = baseline.Execute(new EconomySaveData());
        string b = actual.Execute(new EconomySaveData());
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    
    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        CurrencyTransaction baseline = new CurrencyTransaction(); 
        CurrencyTransaction actual = new CurrencyTransaction();
        //Assert.AreNotEqual(baseline.GetItemId(), actual.GetItemId());
        Assert.AreEqual(baseline.Execute(new EconomySaveData()), actual.Execute(new EconomySaveData()));
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
