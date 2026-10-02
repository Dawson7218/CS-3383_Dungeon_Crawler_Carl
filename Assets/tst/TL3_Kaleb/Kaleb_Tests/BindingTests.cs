using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BindingTests
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        Item baseline = new Item("basicItem", ItemKind.None);
        Item actual = new RelicUpgradeItem("relicItem", "coolRelic", 0.15f, 10);
        string a = baseline.ApplyEffect();
        string b = actual.ApplyEffect();
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        Item baseline = new Item("basicItem", ItemKind.None); 
        Item actual = new Item("basicItem", ItemKind.None);
        //Assert.AreNotEqual(baseline.ApplyEffect(), actual.ApplyEffect());
        Assert.AreEqual(baseline.ApplyEffect(), actual.ApplyEffect());
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
