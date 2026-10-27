using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BindingTests
{
    //instantiating player stats is a temporary bandaid for the dynamic binding tests and showcase
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        Item baseline = new Item("basicItem", ItemKind.None);
        Item actual = new RelicUpgradeItem("relicItem", 0.15f, 10, "coolRelic");
        string a = baseline.ApplyEffect(new PlayerStats());
        string b = actual.ApplyEffect(new PlayerStats());
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        Item baseline = new Item("basicItem", ItemKind.None); 
        Item actual = new Item("basicItem", ItemKind.None);
        //Assert.AreNotEqual(baseline.ApplyEffect(new PlayerStats()), actual.ApplyEffect(new PlayerStats()));
        Assert.AreEqual(baseline.ApplyEffect(new PlayerStats()), actual.ApplyEffect(new PlayerStats()));
    }
}
