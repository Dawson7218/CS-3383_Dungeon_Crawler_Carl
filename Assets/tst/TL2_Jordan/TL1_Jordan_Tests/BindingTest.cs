using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BindingTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void BaseRef_ToSubclass_UsesOverride(){
        Room baseline = new Room();
        Room actual = new CombatRoom();
        string a = baseline.printString();
        string b = actual.printString();
        
        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        Room baseline = new Room(); 
        Room actual = new Room();
        //Assert.AreNotEqual(baseline.printString(), actual.printString());
        Assert.AreEqual(baseline.printString(), actual.printString());
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
