using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Checks dynamic binding: a base-typed reference calls the actual object's Execute method.
public class Getty_Testing_Script
{
    // The plain base object and purchase subclass should return different messages.
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        CurrencyTransaction baseline = new CurrencyTransaction();
        CurrencyTransaction actual = new PurchaseTransaction(StatType.Health, 3);
        string a = baseline.Execute(new EconomySaveData());
        string b = actual.Execute(new EconomySaveData());
        
        // Neither object is using the player's save; these are fresh test data objects.
        Assert.AreNotEqual(a, b);
    }

    
    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        // Both references contain plain CurrencyTransaction objects, so their behavior matches.
        CurrencyTransaction baseline = new CurrencyTransaction(); 
        CurrencyTransaction actual = new CurrencyTransaction();
        Assert.AreEqual(baseline.Execute(new EconomySaveData()), actual.Execute(new EconomySaveData()));
    }
}
