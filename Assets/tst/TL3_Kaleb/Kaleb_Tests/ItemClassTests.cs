using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ItemClassTests
{
    [Test]
    public void Constructors_StoreIdKindAndTexture()
    {
        Item full = new Item("plating", ItemKind.ArmorUpgrade, "armor_plating");
        Assert.AreEqual("plating", full.getItemId());
        Assert.AreEqual(ItemKind.ArmorUpgrade, full.getItemKind());
        Assert.AreEqual("armor_plating", full.getTexture());

        Item twoArgument = new Item("basic", ItemKind.None);
        Assert.AreEqual("basic", twoArgument.getItemId());
        Assert.AreEqual(ItemKind.None, twoArgument.getItemKind());
        Assert.AreEqual("hair_upgrade", twoArgument.getTexture());

        Item empty = new Item();
        Assert.AreEqual("none", empty.getItemId());
        Assert.AreEqual(ItemKind.None, empty.getItemKind());
        Assert.AreEqual("", empty.getTexture());
    }

    [Test]
    public void BaseApplyEffect_AddsMaxHpAndLeavesCurrentHp()
    {
        PlayerStats stats = new PlayerStats();
        string result = new Item("basic", ItemKind.None).ApplyEffect(stats);

        Assert.AreEqual("basicItem", result);
        Assert.AreEqual(1000100, stats.maxHP);
        Assert.AreEqual(100, stats.currentHP);
        Assert.AreEqual(10, stats.attackPower);
        Assert.AreEqual(0, stats.defense);
        Assert.AreEqual(5f, stats.moveSpeed, 0.001f);
        Assert.AreEqual(0.05f, stats.critChance, 0.001f);
    }

    [Test]
    public void WeaponApplyEffect_AddsAttackOnly()
    {
        PlayerStats stats = new PlayerStats();
        WeaponUpgradeItem weapon = new WeaponUpgradeItem("barrel", 4, "barrel");

        Assert.AreEqual(ItemKind.WeaponUpgrade, weapon.getItemKind());
        Assert.AreEqual("weaponUpgradeItem", weapon.ApplyEffect(stats));
        Assert.AreEqual(14, stats.attackPower);
        Assert.AreEqual(100, stats.maxHP);
        Assert.AreEqual(100, stats.currentHP);
        Assert.AreEqual(0, stats.defense);
        Assert.AreEqual(5f, stats.moveSpeed, 0.001f);
        Assert.AreEqual(0.05f, stats.critChance, 0.001f);
    }

    [Test]
    public void ArmorApplyEffect_AddsDefenseAndMaxHp()
    {
        PlayerStats stats = new PlayerStats();
        ArmorUpgradeItem armor = new ArmorUpgradeItem("plating", 3, 7, "plating");

        Assert.AreEqual(ItemKind.ArmorUpgrade, armor.getItemKind());
        Assert.AreEqual("applyEffectItem", armor.ApplyEffect(stats));
        Assert.AreEqual(3, stats.defense);
        Assert.AreEqual(107, stats.maxHP);
        Assert.AreEqual(100, stats.currentHP);
        Assert.AreEqual(10, stats.attackPower);
        Assert.AreEqual(5f, stats.moveSpeed, 0.001f);
        Assert.AreEqual(0.05f, stats.critChance, 0.001f);
    }

    [Test]
    public void ConsumableApplyEffect_AddsAttackAndUniqueId()
    {
        PlayerStats stats = new PlayerStats();
        ConsumableItem consumable = new ConsumableItem("grenade", 10f, 8, "grenade");

        Assert.AreEqual(ItemKind.Consumable, consumable.getItemKind());
        Assert.IsTrue(consumable.getConsumableId().StartsWith("grenade_"));
        Assert.AreEqual("consumableItem", consumable.ApplyEffect(stats));
        Assert.AreEqual(18, stats.attackPower);
        Assert.AreEqual(100, stats.maxHP);
        Assert.AreEqual(0, stats.defense);
        Assert.AreEqual(5f, stats.moveSpeed, 0.001f);
        Assert.AreEqual(0.05f, stats.critChance, 0.001f);
    }

    [Test]
    public void RelicApplyEffect_AddsMoveSpeedAndCrit()
    {
        PlayerStats stats = new PlayerStats();
        RelicUpgradeItem relic = new RelicUpgradeItem("charm", 0.15f, 2, "charm");

        Assert.AreEqual(ItemKind.Relic, relic.getItemKind());
        Assert.IsTrue(relic.getRelicId().StartsWith("charm_"));
        Assert.AreEqual("relicItem", relic.ApplyEffect(stats));
        Assert.AreEqual(5.15f, stats.moveSpeed, 0.001f);
        Assert.AreEqual(2.05f, stats.critChance, 0.001f);
        Assert.AreEqual(10, stats.attackPower);
        Assert.AreEqual(100, stats.maxHP);
        Assert.AreEqual(0, stats.defense);
    }

    [Test]
    public void ItemRef_ApplyEffect_UsesOverride()
    {
        Item weapon = new WeaponUpgradeItem("barrel", 4, "barrel");
        Item armor = new ArmorUpgradeItem("plating", 3, 7, "plating");
        Item consumable = new ConsumableItem("grenade", 10f, 8, "grenade");
        Item relic = new RelicUpgradeItem("charm", 0.15f, 2, "charm");

        Assert.AreEqual("weaponUpgradeItem", weapon.ApplyEffect(new PlayerStats()));
        Assert.AreEqual("applyEffectItem", armor.ApplyEffect(new PlayerStats()));
        Assert.AreEqual("consumableItem", consumable.ApplyEffect(new PlayerStats()));
        Assert.AreEqual("relicItem", relic.ApplyEffect(new PlayerStats()));
    }

    [Test]
    public void BaseClone_KeepsIdAndKind_UsesDefaultTexture()
    {
        Item source = new Item("custom", ItemKind.ArmorUpgrade, "custom_tex");
        Item copy = source.Clone();

        Assert.AreNotSame(source, copy);
        Assert.AreEqual("custom", copy.getItemId());
        Assert.AreEqual(ItemKind.ArmorUpgrade, copy.getItemKind());
        Assert.AreEqual("hair_upgrade", copy.getTexture());
        Assert.AreEqual("custom_tex", source.getTexture());
    }

    [Test]
    public void SubclassClone_CopiesTextureDeltasAndNewIds()
    {
        WeaponUpgradeItem weapon = new WeaponUpgradeItem("barrel", 4, "barrel_tex");
        WeaponUpgradeItem weaponCopy = (WeaponUpgradeItem)weapon.Clone();
        Assert.AreNotSame(weapon, weaponCopy);
        Assert.AreEqual("barrel", weaponCopy.getItemId());
        Assert.AreEqual(ItemKind.WeaponUpgrade, weaponCopy.getItemKind());
        Assert.AreEqual("barrel_tex", weaponCopy.getTexture());
        Assert.AreEqual(4, weaponCopy.getAttackDelta());
        weaponCopy.setAttackDelta(99);
        Assert.AreEqual(4, weapon.getAttackDelta());

        ArmorUpgradeItem armor = new ArmorUpgradeItem("plating", 3, 7, "plating_tex");
        ArmorUpgradeItem armorCopy = (ArmorUpgradeItem)armor.Clone();
        Assert.AreNotSame(armor, armorCopy);
        Assert.AreEqual("plating_tex", armorCopy.getTexture());
        Assert.AreEqual(3, armorCopy.getDefenseDelta());
        Assert.AreEqual(7, armorCopy.getMaxHealthDelta());
        armorCopy.setDefenseDelta(1);
        armorCopy.setMaxHealthDelta(2);
        Assert.AreEqual(3, armor.getDefenseDelta());
        Assert.AreEqual(7, armor.getMaxHealthDelta());

        ConsumableItem consumable = new ConsumableItem("grenade", 10f, 8, "grenade_tex");
        ConsumableItem consumableCopy = (ConsumableItem)consumable.Clone();
        Assert.AreNotSame(consumable, consumableCopy);
        Assert.AreEqual("grenade", consumableCopy.getItemId());
        Assert.AreEqual("grenade_tex", consumableCopy.getTexture());
        Assert.AreEqual(10f, consumableCopy.getDurationRemaining(), 0.001f);
        Assert.AreNotEqual(consumable.getConsumableId(), consumableCopy.getConsumableId());
        Assert.IsTrue(consumableCopy.getConsumableId().StartsWith("grenade_"));
        consumableCopy.setDurationRemaining(1f);
        Assert.AreEqual(10f, consumable.getDurationRemaining(), 0.001f);

        RelicUpgradeItem relic = new RelicUpgradeItem("charm", 0.15f, 2, "charm_tex");
        RelicUpgradeItem relicCopy = (RelicUpgradeItem)relic.Clone();
        Assert.AreNotSame(relic, relicCopy);
        Assert.AreEqual("charm_tex", relicCopy.getTexture());
        Assert.AreEqual(0.15f, relicCopy.getmoveSpeedDelta(), 0.001f);
        Assert.AreNotEqual(relic.getRelicId(), relicCopy.getRelicId());
        Assert.IsTrue(relicCopy.getRelicId().StartsWith("charm_"));
        relicCopy.setmoveSpeedDelta(9f);
        Assert.AreEqual(0.15f, relic.getmoveSpeedDelta(), 0.001f);

        PlayerStats sourceStats = new PlayerStats();
        PlayerStats copyStats = new PlayerStats();
        relic.ApplyEffect(sourceStats);
        relicCopy.ApplyEffect(copyStats);
        Assert.AreEqual(5.15f, sourceStats.moveSpeed, 0.001f);
        Assert.AreEqual(14f, copyStats.moveSpeed, 0.001f);
        Assert.AreEqual(2.05f, sourceStats.critChance, 0.001f);
        Assert.AreEqual(2.05f, copyStats.critChance, 0.001f);
    }

    [Test]
    public void Setters_ChangeTheNextApplyOrRevert()
    {
        PlayerStats weaponStats = new PlayerStats();
        WeaponUpgradeItem weapon = new WeaponUpgradeItem("barrel", 4, "barrel");
        weapon.setAttackDelta(6);
        weapon.ApplyEffect(weaponStats);
        Assert.AreEqual(16, weaponStats.attackPower);

        PlayerStats armorStats = new PlayerStats();
        ArmorUpgradeItem armor = new ArmorUpgradeItem("plating", 3, 7, "plating");
        armor.setDefenseDelta(4);
        armor.setMaxHealthDelta(9);
        armor.ApplyEffect(armorStats);
        Assert.AreEqual(4, armorStats.defense);
        Assert.AreEqual(109, armorStats.maxHP);

        PlayerStats consumableStats = new PlayerStats();
        ConsumableItem consumable = new ConsumableItem("grenade", 10f, 8, "grenade");
        consumable.ApplyEffect(consumableStats);
        consumable.setDurationRemaining(3f);
        consumable.Revert(consumableStats);
        Assert.AreEqual(2.9f, consumable.getDurationRemaining(), 0.001f);
        Assert.AreEqual(18, consumableStats.attackPower);

        PlayerStats relicStats = new PlayerStats();
        RelicUpgradeItem relic = new RelicUpgradeItem("charm", 0.15f, 2, "charm");
        relic.setmoveSpeedDelta(1.5f);
        relic.ApplyEffect(relicStats);
        Assert.AreEqual(6.5f, relicStats.moveSpeed, 0.001f);
        Assert.AreEqual(2.05f, relicStats.critChance, 0.001f);
    }

    [Test]
    public void ApplyEffect_NullStats_ReturnsTypeString()
    {
        Assert.AreEqual("basicItem", new Item("basic", ItemKind.None).ApplyEffect(null));
        Assert.AreEqual("weaponUpgradeItem", new WeaponUpgradeItem("barrel", 4, "barrel").ApplyEffect(null));
        Assert.AreEqual("applyEffectItem", new ArmorUpgradeItem("plating", 3, 7, "plating").ApplyEffect(null));
        Assert.AreEqual("consumableItem", new ConsumableItem("grenade", 10f, 8, "grenade").ApplyEffect(null));
        Assert.AreEqual("relicItem", new RelicUpgradeItem("charm", 0.15f, 2, "charm").ApplyEffect(null));
    }

    [Test]
    public void ZeroDeltas_LeaveStatsAtDefaults()
    {
        PlayerStats weaponStats = new PlayerStats();
        Assert.AreEqual("weaponUpgradeItem", new WeaponUpgradeItem("barrel", 0, "barrel").ApplyEffect(weaponStats));
        AssertDefaults(weaponStats);

        PlayerStats armorStats = new PlayerStats();
        Assert.AreEqual("applyEffectItem", new ArmorUpgradeItem("plating", 0, 0, "plating").ApplyEffect(armorStats));
        AssertDefaults(armorStats);

        PlayerStats consumableStats = new PlayerStats();
        Assert.AreEqual("consumableItem", new ConsumableItem("grenade", 10f, 0, "grenade").ApplyEffect(consumableStats));
        AssertDefaults(consumableStats);

        PlayerStats relicStats = new PlayerStats();
        Assert.AreEqual("relicItem", new RelicUpgradeItem("charm", 0f, 0, "charm").ApplyEffect(relicStats));
        AssertDefaults(relicStats);
    }

    [Test]
    public void NegativeDeltas_ApplyAndClampCurrentHp()
    {
        PlayerStats weaponStats = new PlayerStats();
        new WeaponUpgradeItem("barrel", -30, "barrel").ApplyEffect(weaponStats);
        Assert.AreEqual(-20, weaponStats.attackPower);

        PlayerStats armorStats = new PlayerStats();
        new ArmorUpgradeItem("plating", -2, -50, "plating").ApplyEffect(armorStats);
        Assert.AreEqual(-2, armorStats.defense);
        Assert.AreEqual(50, armorStats.maxHP);
        Assert.AreEqual(50, armorStats.currentHP);
    }

    [Test]
    public void EmptyItemId_StillConstructsAndApplies()
    {
        WeaponUpgradeItem weapon = new WeaponUpgradeItem("", 3, "tex");
        PlayerStats stats = new PlayerStats();

        Assert.AreEqual("", weapon.getItemId());
        Assert.AreEqual(ItemKind.WeaponUpgrade, weapon.getItemKind());
        Assert.AreEqual("weaponUpgradeItem", weapon.ApplyEffect(stats));
        Assert.AreEqual(13, stats.attackPower);
    }

    [Test]
    public void Revert_TicksWhileDurationRemains()
    {
        PlayerStats stats = new PlayerStats();
        ConsumableItem consumable = new ConsumableItem("grenade", 10f, 8, "grenade");
        consumable.ApplyEffect(stats);

        consumable.Revert(stats);

        Assert.AreEqual(9.9f, consumable.getDurationRemaining(), 0.001f);
        Assert.AreEqual(18, stats.attackPower);

        ConsumableItem shortLived = new ConsumableItem("spark", 0.05f, 8, "spark");
        PlayerStats shortStats = new PlayerStats();
        shortLived.ApplyEffect(shortStats);
        shortLived.Revert(shortStats);

        Assert.AreEqual(-0.05f, shortLived.getDurationRemaining(), 0.001f);
        Assert.AreEqual(18, shortStats.attackPower);
    }

    [Test]
    public void Revert_RemovesAttackOnceWhenExpired()
    {
        PlayerStats stats = new PlayerStats();
        ConsumableItem consumable = new ConsumableItem("spark", 0.05f, 8, "spark");
        consumable.ApplyEffect(stats);
        consumable.Revert(stats);

        consumable.Revert(stats);
        Assert.AreEqual(10, stats.attackPower);

        consumable.Revert(stats);
        Assert.AreEqual(10, stats.attackPower);
    }

    [Test]
    public void Revert_NullStats_DoesNotConsumeTheRevert()
    {
        PlayerStats stats = new PlayerStats();
        ConsumableItem consumable = new ConsumableItem("grenade", 0f, 8, "grenade");
        consumable.ApplyEffect(stats);

        Assert.DoesNotThrow(() => consumable.Revert(null));
        Assert.AreEqual(18, stats.attackPower);

        consumable.Revert(stats);
        Assert.AreEqual(10, stats.attackPower);
        consumable.Revert(stats);
        Assert.AreEqual(10, stats.attackPower);
    }

    [UnityTest]
    public IEnumerator Pickup_WithPlayerAndLootSystem_AppliesWeapon()
    {
        if (!Application.isPlaying)
            Assert.Ignore("Pickup destruction is checked in Play Mode.");

        LootSystem previous = LootSystem.Instance;
        SetInstance(null);
        GameObject lootObject = null;
        GameObject playerObject = null;
        GameObject pickupObject = null;
        try
        {
            LootSystem system = MakeLoot(out lootObject);
            playerObject = MakePlayer(out Collider2D collider, out PlayerStats stats);
            WeaponUpgradeItem weapon = new WeaponUpgradeItem("barrel", 4, "barrel");
            InWorldItem pickup = MakePickup(weapon, out pickupObject);

            Enter(pickup, collider);
            yield return null;

            Assert.AreEqual(1, system.Loadout().Items().Count);
            Assert.AreEqual("barrel", system.Loadout().Items()[0].getItemId());
            Assert.AreEqual(14, stats.attackPower);
            Assert.IsTrue(pickupObject == null);
        }
        finally
        {
            SetInstance(previous);
            Destroy(lootObject);
            Destroy(playerObject);
            Destroy(pickupObject);
        }
    }

    [Test]
    public void Pickup_NonPlayer_LeavesPickup()
    {
        LootSystem previous = LootSystem.Instance;
        SetInstance(null);
        GameObject lootObject = null;
        GameObject otherObject = null;
        GameObject pickupObject = null;
        try
        {
            LootSystem system = MakeLoot(out lootObject);
            otherObject = new GameObject("ItemTestOther");
            Collider2D collider = otherObject.AddComponent<BoxCollider2D>();
            InWorldItem pickup = MakePickup(new WeaponUpgradeItem("barrel", 4, "barrel"), out pickupObject);

            Enter(pickup, collider);

            Assert.AreEqual(0, system.Loadout().Items().Count);
            Assert.IsTrue(pickupObject != null);
            Assert.AreEqual("barrel", pickup.Item().getItemId());
        }
        finally
        {
            SetInstance(previous);
            Destroy(lootObject);
            Destroy(otherObject);
            Destroy(pickupObject);
        }
    }

    [Test]
    public void Pickup_MissingLootSystem_LeavesPickup()
    {
        LootSystem previous = LootSystem.Instance;
        SetInstance(null);
        GameObject playerObject = null;
        GameObject pickupObject = null;
        try
        {
            playerObject = MakePlayer(out Collider2D collider, out PlayerStats stats);
            InWorldItem pickup = MakePickup(new WeaponUpgradeItem("barrel", 4, "barrel"), out pickupObject);

            Enter(pickup, collider);

            Assert.AreEqual(10, stats.attackPower);
            Assert.IsTrue(pickupObject != null);
            Assert.AreEqual("barrel", pickup.Item().getItemId());
        }
        finally
        {
            SetInstance(previous);
            Destroy(playerObject);
            Destroy(pickupObject);
        }
    }

    [UnityTest]
    public IEnumerator Pickup_NullItem_LeavesLoadoutEmptyAndDestroys()
    {
        if (!Application.isPlaying)
            Assert.Ignore("Pickup destruction is checked in Play Mode.");

        LootSystem previous = LootSystem.Instance;
        SetInstance(null);
        GameObject lootObject = null;
        GameObject playerObject = null;
        GameObject pickupObject = null;
        try
        {
            LootSystem system = MakeLoot(out lootObject);
            playerObject = MakePlayer(out Collider2D collider, out PlayerStats stats);
            InWorldItem pickup = MakePickup(null, out pickupObject);

            Enter(pickup, collider);
            yield return null;

            Assert.AreEqual(0, system.Loadout().Items().Count);
            Assert.AreEqual(10, stats.attackPower);
            Assert.IsTrue(pickupObject == null);
        }
        finally
        {
            SetInstance(previous);
            Destroy(lootObject);
            Destroy(playerObject);
            Destroy(pickupObject);
        }
    }

    private static void AssertDefaults(PlayerStats stats)
    {
        Assert.AreEqual(100, stats.maxHP);
        Assert.AreEqual(100, stats.currentHP);
        Assert.AreEqual(10, stats.attackPower);
        Assert.AreEqual(0, stats.defense);
        Assert.AreEqual(5f, stats.moveSpeed, 0.001f);
        Assert.AreEqual(0.05f, stats.critChance, 0.001f);
    }

    private static GameObject MakePlayer(out Collider2D collider, out PlayerStats stats)
    {
        GameObject playerObject = new GameObject("ItemTestPlayer");
        collider = playerObject.AddComponent<BoxCollider2D>();
        PlayerController player = playerObject.AddComponent<PlayerController>();
        player.enabled = false;
        InvokeAwake(player);
        stats = player.GetStats();
        return playerObject;
    }

    private static LootSystem MakeLoot(out GameObject lootObject)
    {
        SetInstance(null);
        lootObject = new GameObject("ItemTestLoot");
        LootSystem system = lootObject.AddComponent<LootSystem>();
        InvokeAwake(system);
        return system;
    }

    private static void InvokeAwake(object behaviour)
    {
        MethodInfo awake = behaviour.GetType().GetMethod(
            "Awake",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (awake != null)
            awake.Invoke(behaviour, null);
    }

    private static InWorldItem MakePickup(Item item, out GameObject pickupObject)
    {
        pickupObject = new GameObject("ItemTestPickup");
        InWorldItem pickup = pickupObject.AddComponent<InWorldItem>();
        if (item != null)
        {
            MethodInfo setter = typeof(InWorldItem).GetMethod(
                "SetItem",
                BindingFlags.Instance | BindingFlags.NonPublic);
            setter.Invoke(pickup, new object[] { item });
        }
        return pickup;
    }

    private static void Enter(InWorldItem pickup, Collider2D other)
    {
        MethodInfo trigger = typeof(InWorldItem).GetMethod(
            "OnTriggerEnter2D",
            BindingFlags.Instance | BindingFlags.NonPublic);
        trigger.Invoke(pickup, new object[] { other });
    }

    private static void SetInstance(LootSystem system)
    {
        PropertyInfo property = typeof(LootSystem).GetProperty("Instance");
        MethodInfo setter = property.GetSetMethod(true);
        setter.Invoke(null, new object[] { system });
    }

    private static void Destroy(GameObject gameObject)
    {
        if (gameObject != null)
            Object.DestroyImmediate(gameObject);
    }
}
