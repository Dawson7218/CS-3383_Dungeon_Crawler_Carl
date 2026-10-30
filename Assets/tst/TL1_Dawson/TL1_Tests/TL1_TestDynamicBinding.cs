using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEditor.Progress;

public class TL1_TestDynamicBinding
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        GameState gameState = new GameState();
        GameObject gameObject = new GameObject();

        HudElement baseline = new();
        HudElement actual = new HealthBarElement();
        bool a = baseline.Refresh(gameState, gameObject);
        bool b = actual.Refresh(gameState, gameObject);

        Assert.AreNotEqual(a, b); // assert: DIFFERENT​
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        GameState gameState = new GameState();
        GameObject gameObject = new GameObject();

        HudElement baseline = new();
        HudElement actual = new();
        //Assert.AreNotEqual(baseline.Refresh(gameState), actual.Refresh(gameState));
        Assert.AreEqual(baseline.Refresh(gameState, gameObject), actual.Refresh(gameState, gameObject));
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}
