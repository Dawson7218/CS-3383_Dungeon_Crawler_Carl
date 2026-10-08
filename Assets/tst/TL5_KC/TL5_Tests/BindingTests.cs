using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BindingTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        BossPhase baseline = new BossPhase();
        BossPhase actual = new Phase1_RangedBarrage();
        string a = baseline.CanHandle();
        string b = actual.CanHandle();
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        BossPhase baseline = new BossPhase(); 
        BossPhase actual = new BossPhase();
        // Assert.AreNotEqual(baseline.CanHandle(), actual.CanHandle());
        Assert.AreEqual(baseline.CanHandle(), actual.CanHandle());
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
