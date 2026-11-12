using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

// Every save test uses its own temporary directory, never the player's economy.json.
public class MetaRunIntegrationTests
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private readonly List<GameObject> objects = new List<GameObject>();
    private readonly List<Scene> scenes = new List<Scene>();
    private string directory;
    private string savePath;
    private EconomyManager economy;
    private MetaRunIntegration run;

    private static void Set(object target, string name, object value) =>
        target.GetType().GetField(name, Private).SetValue(target, value);
    private static T Get<T>(object target, string name) =>
        (T)target.GetType().GetField(name, Private).GetValue(target);
    private static object Call(object target, string name, params object[] args) =>
        target.GetType().GetMethod(name, Private).Invoke(target, args);

    private T Component<T>() where T : Component
    {
        var owner = new GameObject(typeof(T).Name + " test");
        owner.SetActive(false);
        objects.Add(owner);
        return owner.AddComponent<T>();
    }

    private EconomyManager LoadEconomy()
    {
        var manager = Component<EconomyManager>();
        Set(manager, "savePathOverride", savePath);
        Call(manager, "Awake");
        return manager;
    }

    [SetUp]
    public void SetUp()
    {
        Assert.IsTrue(EconomyManager.Instance == null, "Run these tests outside an active game.");
        directory = Path.Combine(Path.GetTempPath(), "TL4-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(directory);
        savePath = Path.Combine(directory, "economy.json");
        economy = LoadEconomy();
        run = Component<MetaRunIntegration>();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (run != null) Call(run, "ClearBosses");
        if (economy != null) Call(economy, "OnDestroy");
        foreach (var owner in objects)
            if (owner != null) Object.DestroyImmediate(owner);
        objects.Clear();
        foreach (var scene in scenes)
            if (scene.IsValid() && scene.isLoaded) yield return SceneManager.UnloadSceneAsync(scene);
        scenes.Clear();
        if (directory != null && Directory.Exists(directory)) Directory.Delete(directory, true);
    }

    private void StartTrackedRun(int kills = 0, bool boss = false, float elapsed = 0.25f)
    {
        Set(run, "running", true);
        Set(run, "kills", kills);
        Set(run, "bossKilled", boss);
        Set(run, "startedAt", Time.timeAsDouble - elapsed);
    }

    private void Arrive(string name)
    {
        var scene = SceneManager.CreateScene(name);
        scenes.Add(scene);
        Call(run, "SceneLoaded", scene, LoadSceneMode.Single);
    }

    [TestCase(0, 0, 0, 100, 10, 0)]
    [TestCase(2, 3, 4, 120, 16, 4)]
    public void Upgrades_SetActualCombatStatsAndFullHealth(int health, int attack, int defense,
        int expectedHP, int expectedAttack, int expectedDefense)
    {
        Set(economy, "state", new EconomySaveData { healthLevel = health, attackLevel = attack, defenseLevel = defense });
        var stats = new PlayerStats();
        stats.currentHP = 1;
        economy.ApplyUpgrades(stats);
        Assert.AreEqual(expectedHP, stats.maxHP);
        Assert.AreEqual(expectedHP, stats.currentHP);
        Assert.AreEqual(expectedAttack, stats.attackPower);
        Assert.AreEqual(expectedDefense, stats.defense);
        stats.critChance = 0;
        Assert.AreEqual(expectedAttack, stats.RollAttackDamage(out _));
        stats.TakeDamage(10);
        Assert.AreEqual(expectedHP - Math.Max(1, 10 - expectedDefense), stats.currentHP);
    }

    [Test]
    public void PurchaseAndPayout_ReloadBalanceAndUpgradesFromDisk()
    {
        Assert.IsTrue(economy.TryPurchase(StatType.Health));
        Assert.IsTrue(economy.TryCreditRunEnd(50, 10));
        Object.DestroyImmediate(economy.gameObject);
        economy = LoadEconomy();
        Assert.AreEqual(125, economy.GetBalance());
        var stats = new PlayerStats();
        economy.ApplyUpgrades(stats);
        Assert.AreEqual(110, stats.currentHP);
    }

    [Test]
    public void EndRun_PaysKillsBossAndWholeSecondsOnlyOnce()
    {
        StartTrackedRun(3, true, 12.25f);
        run.EndRun();
        run.EndRun();
        Arrive("DeathScene");
        Assert.AreEqual(242, economy.GetBalance());
        Assert.AreEqual(242, JsonUtility.FromJson<EconomySaveData>(File.ReadAllText(savePath)).points);
        Assert.IsFalse(Get<bool>(run, "payoutPending"));
    }

    [Test]
    public void DeathScene_AutomaticallyPaysAndKeepsBossBonus()
    {
        StartTrackedRun(2, true, 3.25f);
        Arrive("DeathScene");
        Assert.AreEqual(223, economy.GetBalance());
        Assert.IsFalse(Get<bool>(run, "running"));
    }

    [Test]
    public void EndRun_WithoutStartedRun_DoesNotCreateSave()
    {
        run.EndRun();
        Assert.AreEqual(100, economy.GetBalance());
        Assert.IsFalse(File.Exists(savePath));
    }

    [Test]
    public void LeavingForMenu_DoesNotPayOrKeepTracking()
    {
        StartTrackedRun(8, true);
        Arrive("MainMenu");
        run.EndRun();
        Assert.AreEqual(100, economy.GetBalance());
        Assert.IsFalse(Get<bool>(run, "running"));
    }

    [Test]
    public void DefeatNotifications_OnlyCountDuringRun_AndBossDoesNotEndRun()
    {
        Call(run, "EnemyDefeated", new object[] { null });
        Call(run, "BossDefeated");
        Assert.AreEqual(0, Get<int>(run, "kills"));
        Assert.IsFalse(Get<bool>(run, "bossKilled"));
        StartTrackedRun();
        Call(run, "EnemyDefeated", new object[] { null });
        Call(run, "BossDefeated");
        Call(run, "BossDefeated");
        Assert.IsTrue(Get<bool>(run, "running"));
        run.EndRun();
        Assert.AreEqual(210, economy.GetBalance());
    }

    [Test]
    public void FailedSave_KeepsBalanceAndRetriesOnceOnNextScene()
    {
        // A directory at the file path forces a real filesystem failure.
        Directory.CreateDirectory(savePath);
        StartTrackedRun(2, false, 4.25f);
        LogAssert.Expect(LogType.Warning, new Regex(".*"));
        LogAssert.Expect(LogType.Error, new Regex("Run payout was not saved:.*"));
        run.EndRun();
        Assert.AreEqual(100, economy.GetBalance());
        Assert.IsTrue(Get<bool>(run, "payoutPending"));
        Directory.Delete(savePath);
        Arrive("MetaShop");
        run.EndRun();
        Assert.AreEqual(124, economy.GetBalance());
        Assert.IsFalse(Get<bool>(run, "payoutPending"));
    }

    [TestCase(-1f)]
    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    public void InvalidTime_DoesNotSave(float seconds)
    {
        Assert.IsFalse(economy.TryCreditRunEnd(20, seconds));
        Assert.AreEqual(100, economy.GetBalance());
        Assert.IsFalse(File.Exists(savePath));
    }

    [Test]
    public void BeginRun_AppliesOnceAndNextRunResetsCounters()
    {
        var player = Component<PlayerController>();
        player.gameObject.SetActive(true);
        Set(economy, "state", new EconomySaveData { healthLevel = 2, attackLevel = 1 });
        Begin();
        Begin();
        Assert.AreEqual(120, player.GetStats().maxHP);
        StartTrackedRun(5, true);
        run.EndRun();
        Call(player, "Awake"); // A new run gets fresh player stats.
        Begin();
        Assert.AreEqual(120, player.CurrentHP);
        Assert.AreEqual(12, player.GetStats().attackPower);
        Assert.AreEqual(0, Get<int>(run, "kills"));
        Assert.IsFalse(Get<bool>(run, "bossKilled"));
    }

    [Test]
    public void BossCreatedDuringRun_IsSubscribedOnceAndDetachedAtEnd()
    {
        StartTrackedRun();
        var boss = Component<BossController>();
        Call(run, "WatchBosses");
        Call(run, "WatchBosses");
        var field = typeof(BossController).GetField("OnBossDefeated", Private);
        var listeners = (Action)field.GetValue(boss);
        Assert.AreEqual(1, listeners.GetInvocationList().Length);
        listeners.Invoke();
        run.EndRun();
        Assert.AreEqual(200, economy.GetBalance());
        Assert.IsNull(field.GetValue(boss));
    }

    [Test]
    public void UnreadableSave_IsPreservedAndBlocksPayout()
    {
        Object.DestroyImmediate(economy.gameObject);
        File.WriteAllText(savePath, "not valid JSON");
        LogAssert.Expect(LogType.Warning, new Regex(".*"));
        economy = LoadEconomy();
        Assert.IsFalse(economy.TryCreditRunEnd(50, 1));
        Assert.AreEqual("not valid JSON", File.ReadAllText(savePath));
    }

    [Test]
    public void BalanceOverflow_DoesNotWriteOrChangeBalance()
    {
        Set(economy, "state", new EconomySaveData { points = int.MaxValue });
        Assert.IsFalse(economy.TryCreditRunEnd(1, 0));
        Assert.AreEqual(int.MaxValue, economy.GetBalance());
        Assert.IsFalse(File.Exists(savePath));
    }
    private void Begin()
    {
        var routine = (IEnumerator)Call(run, "BeginRun");
        while (routine.MoveNext()) { }
    }
}





