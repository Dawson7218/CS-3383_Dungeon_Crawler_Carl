using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BossPhaseSelectionTests
{
    private GameObject bossObject;
    private BossController boss;
    private BossPhase chain;

    [SetUp]
    public void SetUp()
    {
        bossObject = new GameObject("PhaseSelectionTestBoss");

        // Keep the encounter inactive. These tests only ask the
        // phase chain which phase should handle a supplied state.
        bossObject.SetActive(false);
        boss = bossObject.AddComponent<BossController>();

        BossPhase phase1 = new Phase1_RangedBarrage();
        BossPhase phase2 = new Phase2_SummonMinions();
        BossPhase phase3 = new Phase3_Enrage();

        phase1.SetNext(phase2);
        phase2.SetNext(phase3);

        chain = phase1;
    }

    [TearDown]
    public void TearDown()
    {
        if (bossObject != null)
            Object.DestroyImmediate(bossObject);
    }

    private void SetBossState(
        int maximumHealth,
        int currentHealth,
        bool waveDefeated)
    {
        SetField("maximumHealth", maximumHealth);
        SetField("currentHealth", currentHealth);
        SetField("minionWaveDefeated", waveDefeated);
    }

    private void SetField(string fieldName, object value)
    {
        FieldInfo field = typeof(BossController).GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(
            field,
            Is.Not.Null,
            $"BossController field '{fieldName}' was not found.");

        field.SetValue(boss, value);
    }

    [Test]
    public void BeforeWave_701Health_SelectsPhase1()
    {
        SetBossState(1000, 701, false);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase1_RangedBarrage>());
    }

    [Test]
    public void BeforeWave_Exactly700Health_SelectsPhase2()
    {
        SetBossState(1000, 700, false);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase2_SummonMinions>());
    }

    [Test]
    public void BeforeWave_OneHealth_StillSelectsPhase2()
    {
        // Low health alone must not bypass the mandatory minion wave.
        SetBossState(1000, 1, false);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase2_SummonMinions>());
    }

    [Test]
    public void BeforeWave_ZeroHealth_SelectsNoPhase()
    {
        SetBossState(1000, 0, false);

        Assert.That(chain.Handle(boss), Is.Null);
    }

    [Test]
    public void AfterWave_700Health_SelectsPhase3()
    {
        // Wave completion—not a 35% health check—unlocks Phase 3.
        SetBossState(1000, 700, true);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase3_Enrage>());
    }

    [Test]
    public void AfterWave_350Health_SelectsPhase3()
    {
        SetBossState(1000, 350, true);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase3_Enrage>());
    }

    [Test]
    public void AfterWave_OneHealth_RemainsInPhase3()
    {
        SetBossState(1000, 1, true);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase3_Enrage>());
    }

    [Test]
    public void AfterWave_ZeroHealth_SelectsNoPhase()
    {
        SetBossState(1000, 0, true);

        Assert.That(chain.Handle(boss), Is.Null);
    }

    [Test]
    public void Maximum999_Health700_SelectsPhase1()
    {
        // 70% of 999 is 699.3; the integer threshold is 699.
        SetBossState(999, 700, false);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase1_RangedBarrage>());
    }

    [Test]
    public void Maximum999_Health699_SelectsPhase2()
    {
        SetBossState(999, 699, false);

        Assert.That(
            chain.Handle(boss),
            Is.TypeOf<Phase2_SummonMinions>());
    }
}