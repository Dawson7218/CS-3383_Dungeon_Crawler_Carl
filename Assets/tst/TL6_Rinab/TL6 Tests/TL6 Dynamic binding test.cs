using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TL6Dynamicbindingtest
{
    // A Test behaves as an ordinary method
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        PlayerState baseline = new PlayerState();
        PlayerState actual = new IdleState();
        string a = baseline.Update();
        string b = actual.Update();
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        PlayerState baseline = new PlayerState(); 
        PlayerState actual = new PlayerState();
        //Assert.AreNotEqual(baseline.Update(), actual.Update());
        Assert.AreEqual(baseline.Update(), actual.Update());
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
