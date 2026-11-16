using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossEncounterTests
{
    private GameObject bossObject;
    private GameObject targetObject;
    private GameObject projectileTemplate;
    private GameObject minionTemplate;

    private BossController boss;
    private BossMinionSpawner spawner;
    private SpriteRenderer visual;
    private Transform firePoint;

    private Texture2D texture;
    private Sprite phase1Sprite;
    private Sprite phase3Sprite;

    private float previousTimeScale;

    [SetUp]
    public void SetUp()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 1f;

        // A target position is sufficient: no actual player is needed.
        targetObject = new GameObject("TestTarget");
        targetObject.transform.position = new Vector3(10f, 0f, 0f);

        // Runtime projectile template.
        projectileTemplate = new GameObject("TestProjectileTemplate");
        BossProjectile projectile =
            projectileTemplate.AddComponent<BossProjectile>();

        SetField(projectile, "damageEnabled", false);
        SetField(projectile, "movementEnabled", false);
        SetField(projectile, "autoExpire", false);

        projectileTemplate.GetComponent<Rigidbody2D>().simulated = false;

        // Runtime minion template. EnemyController supplies health;
        // BossMinionAttack disables its ordinary enemy AI.
        minionTemplate = new GameObject("TestMinionTemplate");
        EnemyController minion =
            minionTemplate.AddComponent<EnemyController>();

        minionTemplate.AddComponent<BossMinionAttack>();
        minionTemplate.GetComponent<Rigidbody2D>().simulated = false;

        bossObject = new GameObject("TestBoss");
        boss = bossObject.AddComponent<BossController>();

        // Most tests call public methods directly, without automatic ticks.
        boss.enabled = false;

        spawner = bossObject.AddComponent<BossMinionSpawner>();
        SetField(spawner, "minionPrefab", minion);

        GameObject visualObject = new GameObject("Visual");
        visualObject.transform.SetParent(bossObject.transform, false);
        visual = visualObject.AddComponent<SpriteRenderer>();

        GameObject firePointObject = new GameObject("FirePoint");
        firePointObject.transform.SetParent(bossObject.transform, false);
        firePoint = firePointObject.transform;

        // Two distinct sprites let us verify the Phase 3 sprite swap.
        texture = new Texture2D(2, 2);

        phase1Sprite = Sprite.Create(
            texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));

        phase3Sprite = Sprite.Create(
            texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));

        SetField(boss, "projectilePrefab", projectile);
        SetField(boss, "firePoint", firePoint);
        SetField(boss, "target", targetObject.transform);
        SetField(boss, "bossVisual", visual);
        SetField(boss, "phase1Sprite", phase1Sprite);
        SetField(boss, "phase3Sprite", phase3Sprite);
        SetField(boss, "minionSpawner", spawner);

        // Prevent automatic movement/shots from affecting assertions.
        SetField(boss, "moveSpeed", 0f);
        SetField(boss, "attackInterval", 3600f);
        SetField(boss, "enrageInterval", 3600f);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Clean spawned objects before destroying their owner.
        if (boss != null)
        {
            foreach (BossProjectile shot in Shots())
            {
                if (shot != null)
                    Object.Destroy(shot.gameObject);
            }
        }

        if (spawner != null)
            spawner.ClearMinions();

        if (bossObject != null)
            Object.Destroy(bossObject);

        if (targetObject != null)
            Object.Destroy(targetObject);

        if (projectileTemplate != null)
            Object.Destroy(projectileTemplate);

        if (minionTemplate != null)
            Object.Destroy(minionTemplate);

        if (phase1Sprite != null)
            Object.Destroy(phase1Sprite);

        if (phase3Sprite != null)
            Object.Destroy(phase3Sprite);

        if (texture != null)
            Object.Destroy(texture);

        Time.timeScale = previousTimeScale;
        yield return null;
    }

    private static FieldInfo FindField(object instance, string name)
    {
        FieldInfo field = instance.GetType().GetField(
            name, BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(field, Is.Not.Null, $"Missing field: {name}");
        return field;
    }

    private static void SetField(
        object instance, string name, object value)
    {
        FindField(instance, name).SetValue(instance, value);
    }

    private static T ReadField<T>(object instance, string name)
    {
        return (T)FindField(instance, name).GetValue(instance);
    }

    private List<BossProjectile> Shots()
    {
        return ReadField<List<BossProjectile>>(boss, "projectiles");
    }

    private List<EnemyController> Minions()
    {
        return ReadField<List<EnemyController>>(spawner, "spawnedMinions");
    }

    private void StartPhase2()
    {
        boss.StartEncounter();
        boss.TakeDamage(300);

        Assert.That(
            boss.GetCurrentPhase().PhaseName,
            Is.EqualTo("Phase 2 - Summon Minions"));

        Assert.That(spawner.LivingMinionCount, Is.EqualTo(3));
    }

    private IEnumerator FinishMinionWave()
    {
        for (int i = 0; i < 3; i++)
            spawner.DamageOneLivingMinion(int.MaxValue);

        // Allow the real BossController.Update to detect wave completion.
        boss.enabled = true;

        for (int frame = 0; frame < 30 && !boss.MinionWaveDefeated; frame++)
            yield return null;

        Assert.That(
            boss.MinionWaveDefeated,
            Is.True,
            "The boss did not detect that all minions were defeated.");
    }

    // 1
    [Test]
    public void StartEncounter_ValidSetup_StartsFullHealthPhase1()
    {
        boss.StartEncounter();

        BossPhaseInfo info = boss.GetCurrentPhase();

        Assert.That(info.CurrentHealth, Is.EqualTo(1000));
        Assert.That(info.MaximumHealth, Is.EqualTo(1000));
        Assert.That(info.PhaseName, Is.EqualTo("Phase 1 - Ranged Barrage"));
        Assert.That(info.IsDefeated, Is.False);
        Assert.That(visual.enabled, Is.True);
        Assert.That(visual.sprite, Is.SameAs(phase1Sprite));
    }

    // 2
    [Test]
    public void StartEncounter_CalledAgain_DoesNotResetDamage()
    {
        boss.StartEncounter();
        boss.TakeDamage(100);

        boss.StartEncounter();

        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(900));
        Assert.That(
            boss.GetCurrentPhase().PhaseName,
            Is.EqualTo("Phase 1 - Ranged Barrage"));
    }

    // 3
    [Test]
    public void StartEncounter_MissingTarget_RejectsActivation()
    {
        SetField(boss, "target", null);

        LogAssert.Expect(
            LogType.Error,
            "Boss setup invalid. Check health, projectile, FirePoint, " +
            "target, boss visual, and minion prefab.");

        boss.StartEncounter();
        boss.FireSpread();

        Assert.That(boss.GetCurrentPhase().PhaseName, Is.EqualTo("None"));
        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(0));
        Assert.That(Shots().Count, Is.EqualTo(0));
        Assert.That(spawner.LivingMinionCount, Is.EqualTo(0));
    }

    // 4
    [Test]
    public void TakeDamage_ZeroOrNegative_DoesNotChangeHealth()
    {
        boss.StartEncounter();

        boss.TakeDamage(0);
        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(1000));

        boss.TakeDamage(-100);
        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(1000));
    }

    // 5
    [Test]
    public void LargeHit_EntersMandatoryWave_AndSummonsOnlyOnce()
    {
        boss.StartEncounter();
        boss.TakeDamage(int.MaxValue);

        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(700));
        Assert.That(
            boss.GetCurrentPhase().PhaseName,
            Is.EqualTo("Phase 2 - Summon Minions"));
        Assert.That(visual.enabled, Is.False);
        Assert.That(spawner.LivingMinionCount, Is.EqualTo(3));

        EnemyController[] originalMinions = Minions().ToArray();

        boss.SummonMinions();
        boss.SummonMinions();

        Assert.That(spawner.LivingMinionCount, Is.EqualTo(3));

        // Same objects: repeated calls did not replace the wave either.
        CollectionAssert.AreEqual(originalMinions, Minions());
    }

    // 6
    [Test]
    public void Phase2_IncomingBossDamage_IsIgnored()
    {
        StartPhase2();

        boss.TakeDamage(100);
        boss.TakeDamage(int.MaxValue);

        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(700));
        Assert.That(boss.GetCurrentPhase().IsDefeated, Is.False);
        Assert.That(spawner.LivingMinionCount, Is.EqualTo(3));
    }

    // 7
    [UnityTest]
    public IEnumerator LastMinionDefeated_ActivatesPhase3AndRestoresVisual()
    {
        StartPhase2();

        EnemyController[] originalMinions = Minions().ToArray();

        // Two deaths must not finish the wave.
        spawner.DamageOneLivingMinion(int.MaxValue);
        spawner.DamageOneLivingMinion(int.MaxValue);

        boss.enabled = true;
        yield return null;
        yield return null;

        Assert.That(spawner.LivingMinionCount, Is.EqualTo(1));
        Assert.That(boss.MinionWaveDefeated, Is.False);
        Assert.That(visual.enabled, Is.False);
        Assert.That(
            boss.GetCurrentPhase().PhaseName,
            Is.EqualTo("Phase 2 - Summon Minions"));

        spawner.DamageOneLivingMinion(int.MaxValue);

        for (int frame = 0; frame < 30 && !boss.MinionWaveDefeated; frame++)
            yield return null;

        Assert.That(boss.MinionWaveDefeated, Is.True);
        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(350));
        Assert.That(
            boss.GetCurrentPhase().PhaseName,
            Is.EqualTo("Phase 3 - Enrage"));
        Assert.That(visual.enabled, Is.True);
        Assert.That(visual.sprite, Is.SameAs(phase3Sprite));
        Assert.That(spawner.LivingMinionCount, Is.EqualTo(0));

        yield return null;

        foreach (EnemyController minion in originalMinions)
            Assert.That(minion == null, Is.True);
    }

    // 8
    [Test]
    public void FireSpread_UsesMovedFirePoint_AndThreeAimedDirections()
    {
        boss.StartEncounter();

        bossObject.transform.position = new Vector3(3f, 2f, 0f);
        firePoint.localPosition = new Vector3(0.5f, 0.25f, 0f);

        Vector3 origin = firePoint.position;
        Vector2 aim = ((Vector2)targetObject.transform.position
            - (Vector2)origin).normalized;

        boss.FireSpread();

        Assert.That(Shots().Count, Is.EqualTo(3));

        for (int i = 0; i < 3; i++)
        {
            BossProjectile shot = Shots()[i];

            Assert.That(
                Vector3.Distance(shot.transform.position, origin),
                Is.LessThan(0.001f));

            Vector2 expected =
                Quaternion.Euler(0f, 0f, (i - 1) * 15f)
                * (Vector3)aim;

            Vector2 actual = ReadField<Vector2>(shot, "direction");

            Assert.That(
                Vector2.Distance(actual, expected),
                Is.LessThan(0.001f));
        }
    }

    // 9
    [UnityTest]
    public IEnumerator Phase3_RadialBursts_HaveEightDirectionsAndRotate()
    {
        StartPhase2();
        yield return FinishMinionWave();

        boss.FireRadialBurst();
        boss.FireRadialBurst();

        Assert.That(Shots().Count, Is.EqualTo(16));

        for (int burst = 0; burst < 2; burst++)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = (burst * 22.5f + i * 45f) * Mathf.Deg2Rad;

                Vector2 expected = new Vector2(
                    Mathf.Cos(angle), Mathf.Sin(angle));

                BossProjectile shot = Shots()[burst * 8 + i];
                Vector2 actual = ReadField<Vector2>(shot, "direction");

                Assert.That(
                    Vector2.Distance(actual, expected),
                    Is.LessThan(0.001f));
            }
        }
    }

    // 10
    [UnityTest]
    public IEnumerator Defeat_SignalsOnce_ClearsShotsAndPreventsRestart()
    {
        StartPhase2();
        yield return FinishMinionWave();

        int notifications = 0;
        boss.OnBossDefeated += () => notifications++;

        boss.FireRadialBurst();
        BossProjectile[] existingShots = Shots().ToArray();

        Assert.That(existingShots.Length, Is.EqualTo(8));

        boss.TakeDamage(int.MaxValue);

        // Repeated damage and calls after defeat must not restart combat.
        boss.TakeDamage(100);
        boss.StartEncounter();
        boss.FireSpread();
        boss.FireRadialBurst();
        boss.SummonMinions();

        Assert.That(notifications, Is.EqualTo(1));
        Assert.That(boss.GetCurrentPhase().CurrentHealth, Is.EqualTo(0));
        Assert.That(boss.GetCurrentPhase().IsDefeated, Is.True);
        Assert.That(boss.GetCurrentPhase().PhaseName, Is.EqualTo("None"));
        Assert.That(Shots().Count, Is.EqualTo(0));
        Assert.That(spawner.LivingMinionCount, Is.EqualTo(0));

        yield return null;

        foreach (BossProjectile shot in existingShots)
            Assert.That(shot == null, Is.True);
    }
}