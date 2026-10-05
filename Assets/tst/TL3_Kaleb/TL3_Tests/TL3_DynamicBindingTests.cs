using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TL3_DynamicBindingTests
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        Item baseline = new Item();
        Item actual = new RelicItem();

        string a = baseline.ItemId();
        string b = actual.ItemId();

        Assert.AreNotEqual(a,b);
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        Item baseline = new Item();
        Item actual = new Item();

        string a = baseline.ItemId();
        string b = actual.ItemId();

        //Assert.AreNotEqual(a,b);
        Assert.AreEqual(a,b);
    }
}
