using NUnit.Framework;

public class BindingTests
{
    // A test-only phase that records whether its override ran.
    private class TestPhase : BossPhase
    {
        private readonly bool canHandle;

        public bool WasExecuted { get; private set; }

        public override string PhaseName => "Test Phase";

        public TestPhase(bool canHandle)
        {
            this.canHandle = canHandle;
        }

        public override bool CanHandle(BossController ctx)
        {
            return canHandle;
        }

        public override void ExecutePhase(BossController ctx)
        {
            WasExecuted = true;
        }
    }

    [Test]
    public void BaseReference_ExecutePhase_UsesDerivedOverride()
    {
        TestPhase concretePhase = new TestPhase(true);
        BossPhase phase = concretePhase;

        phase.ExecutePhase(null);

        Assert.That(concretePhase.WasExecuted, Is.True);
    }

    [Test]
    public void Handle_WhenCurrentPhaseAccepts_ReturnsCurrentPhase()
    {
        BossPhase first = new TestPhase(true);
        BossPhase second = new TestPhase(true);
        first.SetNext(second);

        BossPhase selected = first.Handle(null);

        Assert.That(selected, Is.SameAs(first));
    }

    [Test]
    public void Handle_WhenCurrentPhaseRejects_PassesToNextPhase()
    {
        BossPhase first = new TestPhase(false);
        BossPhase second = new TestPhase(true);
        first.SetNext(second);

        BossPhase selected = first.Handle(null);

        Assert.That(selected, Is.SameAs(second));
    }

    [Test]
    public void Handle_WhenNoPhaseAccepts_ReturnsNull()
    {
        BossPhase first = new TestPhase(false);
        BossPhase second = new TestPhase(false);
        first.SetNext(second);

        BossPhase selected = first.Handle(null);

        Assert.That(selected, Is.Null);
    }
}