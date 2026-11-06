using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class LootSystemTests
{
    private const string PrefabPath = "Assets/prefabs/TL3_Kaleb/LootSystem.prefab";
    private const int RollSeed = 12345;

    private static readonly string[] CatalogIds =
    {
        "armor_plating",
        "armor_upgrade_1",
        "armor_upgrade_2",
        "armor_upgrade_4",
        "armor_upgrade_5",
        "grenade_upgrade_1",
        "grenade_upgrade_2",
        "temp_dmg_upgrade_1",
        "hair_upgrade",
        "luck_upgrade",
        "sight_upgrade_1",
        "sight_upgrade_2",
        "sight_upgrade_3",
        "speed_upgrade_1",
        "speed_upgrade_2",
        "barrel_extension",
        "bullet_upgrade_2",
        "bullet_upgrade_3"
    };

    private static readonly string[] CombatIds =
    {
        "armor_plating",
        "armor_upgrade_1",
        "armor_upgrade_2",
        "armor_upgrade_4",
        "barrel_extension",
        "bullet_upgrade_2",
        "sight_upgrade_1",
        "temp_dmg_upgrade_1"
    };

    private static readonly string[] TreasureIds =
    {
        "armor_upgrade_4",
        "armor_upgrade_5",
        "grenade_upgrade_1",
        "grenade_upgrade_2",
        "hair_upgrade",
        "bullet_upgrade_3",
        "sight_upgrade_2",
        "sight_upgrade_3",
        "speed_upgrade_1",
        "speed_upgrade_2"
    };

    private static readonly string[] CorridorIds =
    {
        "temp_dmg_upgrade_1",
        "armor_plating"
    };

    [Test]
    public void Table_PrefabDropRates_MatchSerializedChances()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            List<LootTable.RoomDropRate> rates = table.DropRates();

            Assert.AreEqual(5, rates.Count);
            AssertRate(rates[0], RoomType.None, 0f);
            AssertRate(rates[1], RoomType.CombatRoom, 0.75f);
            AssertRate(rates[2], RoomType.TreasureRoom, 1f);
            AssertRate(rates[3], RoomType.CorridorRoom, 0.25f);
            AssertRate(rates[4], RoomType.BossRoom, 0f);
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_Prototypes_ListsCatalogAndIsShared()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            List<Item> prototypes = table.Prototypes();
            HashSet<string> ids = new HashSet<string>();
            foreach (Item item in prototypes)
                ids.Add(item.getItemId());

            Assert.AreEqual(CatalogIds.Length, prototypes.Count);
            foreach (string id in CatalogIds)
                Assert.IsTrue(ids.Contains(id), id);
            Assert.AreSame(prototypes, new LootTable().Prototypes());
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_TreasureRoom_AlwaysDropsACloneFromItsTable()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            AssertDrops(table, RoomType.TreasureRoom, 200, TreasureIds, 200, 200);
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_CombatRoom_DropFractionStaysNearPrefabChance()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            AssertDrops(table, RoomType.CombatRoom, 400, CombatIds, 240, 360);
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_CorridorRoom_DropFractionStaysNearPrefabChance()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            AssertDrops(table, RoomType.CorridorRoom, 400, CorridorIds, 50, 160);
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_NoneAndBoss_AlmostNeverDrop()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            string[] hair = { "hair_upgrade" };
            AssertDrops(table, RoomType.None, 200, hair, 0, 2);
            AssertDrops(table, RoomType.BossRoom, 200, hair, 0, 2);
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_MissingRoomRate_ReturnsNull()
    {
        LootTable table = new LootTable();
        foreach (RoomType roomType in Enum.GetValues(typeof(RoomType)))
            Assert.IsNull(table.RollDrop(roomType));
    }

    [Test]
    public void Table_ClearedTreasureWeights_ReturnsNull()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            Rows(table, RoomType.TreasureRoom).Clear();
            WithSeed(RollSeed, () => Assert.IsNull(table.RollDrop(RoomType.TreasureRoom)));
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_UnknownItemId_ReturnsNull()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            IList rows = Rows(table, RoomType.TreasureRoom);
            foreach (object row in rows)
                SetSpawnId(row, "not_a_real_item");

            WithSeed(RollSeed, () => Assert.IsNull(table.RollDrop(RoomType.TreasureRoom)));
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Table_ZeroWeights_ReturnsNull()
    {
        LootSystem previous = null;
        GameObject instance = null;
        try
        {
            LootTable table = PrefabTable(out instance, out previous);
            foreach (object row in Rows(table, RoomType.TreasureRoom))
                SetWeight(row, 0);

            WithSeed(RollSeed, () => Assert.IsNull(table.RollDrop(RoomType.TreasureRoom)));
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void Awake_OnPrefab_SetsInstanceTableAndLoadout()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject instance = null;
        try
        {
            LootSystem system = CreatePrefabInstance(out instance);
            EnsureAwake(system);

            Assert.AreSame(system, LootSystem.Instance);
            Assert.IsNotNull(system.Table());
            Assert.IsNotNull(system.Loadout());
            Assert.AreEqual(5, system.Table().DropRates().Count);
            Assert.AreEqual(0.75f, system.Table().DropRates()[1].GetDropRate(), 0.001f);
            Assert.AreSame(system.Table().Prototypes(), new LootTable().Prototypes());
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [UnityTest]
    public IEnumerator Awake_DuplicateInstance_KeepsTheFirst()
    {
        if (!Application.isPlaying)
            Assert.Ignore("Duplicate destruction is checked in Play Mode.");

        LootSystem previous = LootSystem.Instance;
        SetInstance(null);
        GameObject firstObject = null;
        GameObject secondObject = null;
        try
        {
            firstObject = UnityEngine.Object.Instantiate(PrefabAsset());
            secondObject = UnityEngine.Object.Instantiate(PrefabAsset());
            yield return null;

            Assert.IsTrue(firstObject != null);
            Assert.AreSame(firstObject.GetComponent<LootSystem>(), LootSystem.Instance);
            Assert.IsTrue(secondObject == null);
        }
        finally
        {
            SetInstance(previous);
            Destroy(firstObject);
            Destroy(secondObject);
        }
    }

    [Test]
    public void ApplyItem_WeaponRaisesAttack()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject host = null;
        try
        {
            LootSystem system = CreateBare(out host);
            PlayerStats stats = new PlayerStats();
            WeaponUpgradeItem weapon = new WeaponUpgradeItem("barrel", 4, "barrel");

            system.ApplyItem(weapon, stats);

            Assert.AreEqual(14, stats.attackPower);
            Assert.AreEqual(100, stats.maxHP);
            Assert.AreEqual(100, stats.currentHP);
            Assert.AreEqual(0, stats.defense);
            Assert.AreEqual(5f, stats.moveSpeed, 0.001f);
            Assert.AreEqual(0.05f, stats.critChance, 0.001f);
            Assert.AreEqual(1, system.Loadout().Items().Count);
            Assert.AreSame(weapon, system.Loadout().Items()[0]);
        }
        finally
        {
            SetInstance(previous);
            Destroy(host);
        }
    }

    [Test]
    public void ApplyItem_NullItemAndNullStats_LeaveLoadoutAndFreshStatsUnchanged()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject host = null;
        try
        {
            LootSystem system = CreateBare(out host);
            PlayerStats stats = new PlayerStats();

            system.ApplyItem(null, stats);
            system.ApplyItem(null, null);

            Assert.AreEqual(0, system.Loadout().Items().Count);
            AssertDefaults(stats);
        }
        finally
        {
            SetInstance(previous);
            Destroy(host);
        }
    }

    [Test]
    public void GenerateLootAt_SpawnsStandInAtLocation()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject instance = null;
        GameObject standIn = null;
        GameObject spawned = null;
        Vector2 location = new Vector2(813.5f, -642.25f);
        try
        {
            LootSystem system = CreatePrefabInstance(out instance);
            standIn = new GameObject("LootTestStandIn");
            standIn.AddComponent<InWorldItem>();
            standIn.AddComponent<SpriteRenderer>();
            standIn.transform.position = new Vector3(-50f, -50f, 0f);
            SetItemPrefab(system, standIn);

            Item expected = null;
            WithSeed(RollSeed, () => expected = system.Table().RollDrop(RoomType.TreasureRoom));
            WithSeed(RollSeed, () => system.GenerateLootAt(RoomType.TreasureRoom, location));

            spawned = FindOtherAt(location, standIn);
            Assert.IsNotNull(expected);
            Assert.IsNotNull(spawned);
            InWorldItem worldItem = spawned.GetComponent<InWorldItem>();
            Assert.IsNotNull(worldItem);
            Assert.IsNotNull(worldItem.Item());
            Assert.AreEqual(expected.getItemId(), worldItem.Item().getItemId());
            Assert.AreNotSame(expected, worldItem.Item());
            Assert.AreEqual(location.x, spawned.transform.position.x, 0.001f);
            Assert.AreEqual(location.y, spawned.transform.position.y, 0.001f);
            Assert.AreEqual(0f, spawned.transform.position.z, 0.001f);

            Sprite loaded = Resources.Load<Sprite>("TL3_Kaleb/" + worldItem.Item().getTexture());
            Assert.IsNotNull(loaded);
            Assert.AreEqual(loaded, spawned.GetComponent<SpriteRenderer>().sprite);
        }
        finally
        {
            SetInstance(previous);
            Destroy(spawned);
            Destroy(standIn);
            Destroy(instance);
        }
    }

    [Test]
    public void GenerateLootAt_NullPrefab_SpawnsNothing()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject instance = null;
        Vector2 location = new Vector2(814.5f, -643.25f);
        try
        {
            LootSystem system = CreatePrefabInstance(out instance);
            Assert.IsNull(FindOtherAt(location, null));

            Assert.DoesNotThrow(() => system.GenerateLootAt(RoomType.TreasureRoom, location));

            Assert.IsNull(FindOtherAt(location, null));
        }
        finally
        {
            SetInstance(previous);
            Destroy(instance);
        }
    }

    [Test]
    public void GenerateLootAt_NullTable_SpawnsNothing()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject host = null;
        Vector2 location = new Vector2(815.5f, -644.25f);
        try
        {
            LootSystem system = CreateBare(out host);
            Assert.IsNull(system.Table());
            Assert.IsNull(FindOtherAt(location, null));

            Assert.DoesNotThrow(() => system.GenerateLootAt(RoomType.TreasureRoom, location));

            Assert.IsNull(FindOtherAt(location, null));
        }
        finally
        {
            SetInstance(previous);
            Destroy(host);
        }
    }

    [Test]
    public void GenerateLootAt_StandInWithoutItemComponent_DoesNotThrow()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject instance = null;
        GameObject standIn = null;
        GameObject spawned = null;
        Vector2 location = new Vector2(816.5f, -645.25f);
        try
        {
            LootSystem system = CreatePrefabInstance(out instance);
            standIn = new GameObject("LootTestBareStandIn");
            standIn.transform.position = new Vector3(-60f, -60f, 0f);
            SetItemPrefab(system, standIn);

            Assert.DoesNotThrow(() => system.GenerateLootAt(RoomType.TreasureRoom, location));

            spawned = FindOtherAt(location, standIn);
            Assert.IsNotNull(spawned);
            Assert.IsNull(spawned.GetComponent<InWorldItem>());
            Assert.IsNull(spawned.GetComponent<SpriteRenderer>());
        }
        finally
        {
            SetInstance(previous);
            Destroy(spawned);
            Destroy(standIn);
            Destroy(instance);
        }
    }

    [Test]
    public void DeprecatedRollDrop_ReturnsSteakRelic()
    {
        LootSystem previous = LootSystem.Instance;
        GameObject host = null;
        try
        {
            LootSystem system = CreateBare(out host);
            Item item = system.RollDrop(RoomType.CorridorRoom);

            Assert.IsInstanceOf<RelicUpgradeItem>(item);
            Assert.AreEqual("steak", item.getItemId());
        }
        finally
        {
            SetInstance(previous);
            Destroy(host);
        }
    }

    private static void AssertRate(LootTable.RoomDropRate rate, RoomType roomType, float dropRate)
    {
        Assert.AreEqual(roomType, rate.GetRoomType());
        Assert.AreEqual(dropRate, rate.GetDropRate(), 0.001f);
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

    private static void AssertDrops(
        LootTable table,
        RoomType roomType,
        int rolls,
        string[] allowedIds,
        int minNonNull,
        int maxNonNull)
    {
        HashSet<string> allowed = new HashSet<string>(allowedIds);
        WithSeed(RollSeed, () =>
        {
            int nonNull = 0;
            for (int i = 0; i < rolls; i++)
            {
                Item drop = table.RollDrop(roomType);
                if (drop == null)
                    continue;

                nonNull++;
                Assert.IsTrue(allowed.Contains(drop.getItemId()), drop.getItemId());
                Item prototype = FindPrototype(table, drop.getItemId());
                Assert.IsNotNull(prototype);
                Assert.AreNotSame(prototype, drop);
            }

            Assert.GreaterOrEqual(nonNull, minNonNull);
            Assert.LessOrEqual(nonNull, maxNonNull);
        });
    }

    private static Item FindPrototype(LootTable table, string id)
    {
        foreach (Item item in table.Prototypes())
        {
            if (item.getItemId() == id)
                return item;
        }

        return null;
    }

    private static void WithSeed(int seed, Action body)
    {
        UnityEngine.Random.State previous = UnityEngine.Random.state;
        try
        {
            UnityEngine.Random.InitState(seed);
            body();
        }
        finally
        {
            UnityEngine.Random.state = previous;
        }
    }

    private static LootTable PrefabTable(out GameObject instance, out LootSystem previous)
    {
        previous = LootSystem.Instance;
        return CreatePrefabInstance(out instance).Table();
    }

    private static LootSystem CreatePrefabInstance(out GameObject instance)
    {
        SetInstance(null);
        instance = UnityEngine.Object.Instantiate(PrefabAsset());
        instance.name = "LootSystemTestInstance";
        return instance.GetComponent<LootSystem>();
    }

    private static LootSystem CreateBare(out GameObject host)
    {
        SetInstance(null);
        host = new GameObject("LootSystemTestBare");
        return host.AddComponent<LootSystem>();
    }

    private static GameObject PrefabAsset()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Assert.IsNotNull(prefab, PrefabPath);
        return prefab;
    }

    private static IList Rows(LootTable table, RoomType roomType)
    {
        FieldInfo weightsField = typeof(LootTable).GetField(
            "weights",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(weightsField);
        IEnumerable weights = (IEnumerable)weightsField.GetValue(table);
        Assert.IsNotNull(weights);

        foreach (object roomTable in weights)
        {
            MethodInfo getRoomType = roomTable.GetType().GetMethod("GetRoomType");
            RoomType found = (RoomType)getRoomType.Invoke(roomTable, null);
            if (found != roomType)
                continue;

            MethodInfo getLootTable = roomTable.GetType().GetMethod("GetLootTable");
            IList rows = (IList)getLootTable.Invoke(roomTable, null);
            Assert.IsNotNull(rows);
            return rows;
        }

        Assert.Fail("No weight rows for " + roomType);
        return null;
    }

    private static void SetSpawnId(object row, string id)
    {
        FieldInfo field = row.GetType().GetField(
            "SpawnableItem",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(row, id);
    }

    private static void SetWeight(object row, int weight)
    {
        FieldInfo field = row.GetType().GetField(
            "dropWeight",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(row, weight);
    }

    private static void SetItemPrefab(LootSystem system, GameObject prefab)
    {
        FieldInfo field = typeof(LootSystem).GetField(
            "itemPrefabBase",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        field.SetValue(system, prefab);
    }

    private static GameObject FindOtherAt(Vector2 location, GameObject except)
    {
        foreach (Transform transform in UnityEngine.Object.FindObjectsByType<Transform>())
        {
            if (except != null && transform.gameObject == except)
                continue;
            if (Mathf.Abs(transform.position.x - location.x) > 0.001f)
                continue;
            if (Mathf.Abs(transform.position.y - location.y) > 0.001f)
                continue;
            return transform.gameObject;
        }

        return null;
    }

    private static void EnsureAwake(LootSystem system)
    {
        if (LootSystem.Instance == system)
            return;

        MethodInfo awake = typeof(LootSystem).GetMethod(
            "Awake",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(awake);
        awake.Invoke(system, null);
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
            UnityEngine.Object.DestroyImmediate(gameObject);
    }
}
