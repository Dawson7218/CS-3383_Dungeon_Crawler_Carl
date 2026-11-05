using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ItemMementoTests
{
    [Test]
    public void RoundTrip_RestoresIdsStatsAndConsumableTime()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        ConsumableItem grenade = Grenade(7.5f);
        string originalConsumableId = grenade.getConsumableId();

        loadout.ApplyItem(Armor(), stats);
        loadout.ApplyItem(grenade, stats);
        ItemMemento checkpoint = loadout.CreateMemento();

        loadout.ApplyItem(Barrel(), stats);
        stats.ModifyAttackPower(15);
        stats.TakeDamage(20);

        loadout.SetMemento(checkpoint);

        Assert.AreEqual(2, loadout.Items().Count);
        Assert.AreEqual("armor_plating", loadout.Items()[0].getItemId());
        Assert.AreEqual("grenade_upgrade_1", loadout.Items()[1].getItemId());

        ConsumableItem restored = (ConsumableItem)loadout.Items()[1];
        Assert.AreEqual(7.5f, restored.getDurationRemaining(), 0.001f);
        Assert.AreEqual(20, stats.attackPower);
        Assert.AreEqual(5, stats.defense);
        Assert.AreEqual(102, stats.maxHP);
        Assert.AreNotSame(grenade, restored);
        Assert.AreNotEqual(originalConsumableId, restored.getConsumableId());
        Assert.AreEqual(restored.getConsumableId(), loadout.ConsumableTimers()[0].Item1);
        Assert.AreEqual(7.5f, loadout.ConsumableTimers()[0].Item2, 0.001f);
    }

    [Test]
    public void Snapshot_IsIndependentOfLaterChanges()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        ConsumableItem grenade = Grenade(7.5f);

        loadout.ApplyItem(Armor(), stats);
        loadout.ApplyItem(grenade, stats);
        ItemMemento checkpoint = loadout.CreateMemento();
        int savedAttack = stats.attackPower;
        int savedCurrentHp = stats.currentHP;
        int savedDefense = stats.defense;
        int savedMaxHp = stats.maxHP;

        grenade.setDurationRemaining(1f);
        stats.ModifyAttackPower(50);
        stats.TakeDamage(30);

        loadout.SetMemento(checkpoint);

        ConsumableItem restored = (ConsumableItem)loadout.Items()[1];
        Assert.AreEqual(7.5f, restored.getDurationRemaining(), 0.001f);
        Assert.AreEqual(savedAttack, stats.attackPower);
        Assert.AreEqual(savedCurrentHp, stats.currentHP);
        Assert.AreEqual(savedDefense, stats.defense);
        Assert.AreEqual(savedMaxHp, stats.maxHP);
    }

    [Test]
    public void TwoCheckpoints_RestoreIndependently()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();

        loadout.ApplyItem(Armor(), stats);
        ItemMemento first = loadout.CreateMemento();

        loadout.ApplyItem(Grenade(7.5f), stats);
        ItemMemento second = loadout.CreateMemento();

        loadout.ApplyItem(Barrel(), stats);

        loadout.SetMemento(first);
        CollectionAssert.AreEqual(new[] { "armor_plating" }, Ids(loadout));
        Assert.AreEqual(10, stats.attackPower);
        Assert.AreEqual(5, stats.defense);

        loadout.SetMemento(second);
        CollectionAssert.AreEqual(new[] { "armor_plating", "grenade_upgrade_1" }, Ids(loadout));
        Assert.AreEqual(20, stats.attackPower);

        loadout.SetMemento(first);
        CollectionAssert.AreEqual(new[] { "armor_plating" }, Ids(loadout));
        Assert.AreEqual(10, stats.attackPower);
    }

    [Test]
    public void Restore_DoesNotReapplyEffects()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        loadout.ApplyItem(Armor(), stats);
        loadout.ApplyItem(Grenade(7.5f), stats);

        int savedAttack = stats.attackPower;
        int savedDefense = stats.defense;
        int savedMaxHp = stats.maxHP;
        ItemMemento checkpoint = loadout.CreateMemento();

        loadout.ApplyItem(Barrel(), stats);
        loadout.ApplyItem(Armor(), stats);
        Assert.AreNotEqual(savedAttack, stats.attackPower);
        Assert.AreNotEqual(savedDefense, stats.defense);
        Assert.AreNotEqual(savedMaxHp, stats.maxHP);

        loadout.SetMemento(checkpoint);

        Assert.AreEqual(savedAttack, stats.attackPower);
        Assert.AreEqual(savedDefense, stats.defense);
        Assert.AreEqual(savedMaxHp, stats.maxHP);
        Assert.AreNotEqual(savedAttack + 10, stats.attackPower);
        Assert.AreNotEqual(savedDefense + 5, stats.defense);
        Assert.AreNotEqual(savedMaxHp + 2, stats.maxHP);
    }

    [Test]
    public void NullRestore_LeavesLoadoutAlone()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        loadout.ApplyItem(Armor(), stats);
        int attack = stats.attackPower;

        loadout.SetMemento(null);

        Assert.AreEqual(1, loadout.Items().Count);
        Assert.AreEqual("armor_plating", loadout.Items()[0].getItemId());
        Assert.AreEqual(attack, stats.attackPower);

        GameObject go = new GameObject("LootSystemNullRestore");
        LootSystem system = go.AddComponent<LootSystem>();
        try
        {
            PlayerStats caretakerStats = new PlayerStats();
            system.ApplyItem(Armor(), caretakerStats);
            int defense = caretakerStats.defense;

            system.RestoreState(null);

            Assert.AreEqual(1, system.Loadout().Items().Count);
            Assert.AreEqual("armor_plating", system.Loadout().Items()[0].getItemId());
            Assert.AreEqual(defense, caretakerStats.defense);
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void UnknownId_IsDroppedAndStatsStayAtCheckpoint()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        loadout.ApplyItem(Armor(), stats);
        loadout.ApplyItem(new WeaponUpgradeItem("not_in_table", 7, "missing"), stats);
        ItemMemento checkpoint = loadout.CreateMemento();

        stats.ModifyAttackPower(3);
        loadout.SetMemento(checkpoint);

        Assert.AreEqual(1, loadout.Items().Count);
        Assert.AreEqual("armor_plating", loadout.Items()[0].getItemId());
        Assert.AreEqual(17, stats.attackPower);
        Assert.AreEqual(5, stats.defense);
        Assert.AreEqual(102, stats.maxHP);
    }

    [Test]
    public void Caretaker_SaveStateAndSaveAState_RoundTrip()
    {
        GameObject go = new GameObject("LootSystemCaretaker");
        LootSystem system = go.AddComponent<LootSystem>();
        try
        {
            PlayerStats stats = new PlayerStats();
            system.ApplyItem(Armor(), stats);
            ItemMemento saved = system.SaveState();

            system.ApplyItem(Barrel(), stats);
            system.RestoreState(saved);

            Assert.AreEqual(1, system.Loadout().Items().Count);
            Assert.AreEqual("armor_plating", system.Loadout().Items()[0].getItemId());
            Assert.AreEqual(10, stats.attackPower);
            Assert.AreEqual(5, stats.defense);
            Assert.AreEqual(102, stats.maxHP);

            system.ApplyItem(Barrel(), stats);
            ItemMemento savedAgain = system.SaveAState();
            system.ApplyItem(Grenade(7.5f), stats);
            system.RestoreState(savedAgain);

            CollectionAssert.AreEqual(
                new[] { "armor_plating", "barrel_extension" },
                Ids(system.Loadout()));
            Assert.AreEqual(20, stats.attackPower);
            Assert.AreEqual(5, stats.defense);
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void EmptySave_BeforeApplyItem_ClearsItemsAndLeavesStats()
    {
        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        ItemMemento checkpoint = loadout.CreateMemento();

        loadout.ApplyItem(Armor(), stats);
        loadout.SetMemento(checkpoint);

        Assert.AreEqual(0, loadout.Items().Count);
        Assert.AreEqual(5, stats.defense);
        Assert.AreEqual(102, stats.maxHP);
        Assert.AreEqual(10, stats.attackPower);
    }

    [Test]
    public void Memento_HasNoPublicSnapshotSurface()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
        Assert.AreEqual(0, typeof(ItemMemento).GetProperties(flags).Length);
        Assert.AreEqual(0, typeof(ItemMemento).GetMethods(flags).Length);
        Assert.AreEqual(0, typeof(ItemMemento).GetFields(flags).Length);

        Loadout loadout = new Loadout();
        PlayerStats stats = new PlayerStats();
        loadout.ApplyItem(Armor(), stats);
        ItemMemento checkpoint = loadout.CreateMemento();
        Assert.IsNotNull(checkpoint);

        loadout.ApplyItem(Barrel(), stats);
        loadout.SetMemento(checkpoint);
        Assert.AreEqual(1, loadout.Items().Count);
        Assert.AreEqual("armor_plating", loadout.Items()[0].getItemId());
    }

    private static ArmorUpgradeItem Armor()
    {
        return new ArmorUpgradeItem("armor_plating", 5, 2, "armor_plating");
    }

    private static WeaponUpgradeItem Barrel()
    {
        return new WeaponUpgradeItem("barrel_extension", 10, "barrel_extension");
    }

    private static ConsumableItem Grenade(float remaining)
    {
        ConsumableItem grenade = new ConsumableItem("grenade_upgrade_1", 20f, 10, "grenade_upgrade_1");
        grenade.setDurationRemaining(remaining);
        return grenade;
    }

    private static List<string> Ids(Loadout loadout)
    {
        List<string> ids = new List<string>();
        foreach (Item item in loadout.Items())
            ids.Add(item.getItemId());
        return ids;
    }
}
