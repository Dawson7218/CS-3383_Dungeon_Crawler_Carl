using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BindingTests
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        Item baseline = new Item();
        Item actual = new RelicUpgradeItem();
        string a = baseline.GetItemId();
        string b = actual.GetItemId();
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        Item baseline = new Item(); 
        Item actual = new Item();
        //Assert.AreNotEqual(baseline.GetItemId(), actual.GetItemId());
        Assert.AreEqual(baseline.GetItemId(), actual.GetItemId());
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
