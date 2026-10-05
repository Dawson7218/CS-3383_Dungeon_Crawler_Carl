using NUnit.Framework;

public class BindingTests
{
    [Test]
    public void BaseRef_ToSubclass_UsesOverride()
    {
        EnemyState baseline = new IdleState();
        EnemyState actual = new ChaseState();
        string a = baseline.Handle(null);
        string b = actual.Handle(null);

        Assert.AreNotEqual(a, b); // assert: DIFFERENT
    }

    [Test]
    public void BaseRef_ToBase_NoBindingChange()
    {
        EnemyState baseline = new IdleState();
        EnemyState actual = new IdleState();
        //Assert.AreNotEqual(baseline.Handle(null), actual.Handle(null));
        Assert.AreEqual(baseline.Handle(null), actual.Handle(null));
        // Run 1: RED - same output, nothing to bind to
        // Run 2: change to Assert.AreEqual -> GREEN -> commit
    }
}