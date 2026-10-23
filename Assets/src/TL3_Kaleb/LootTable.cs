using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[Serializable]
public class LootTable
{
    static private List<Item> prototypes = new List<Item>
    {
        new ArmorUpgradeItem("armor_plating", 5, 2, "armor_plating"),
        new ArmorUpgradeItem("armor_upgrade_1", 5, 2, "armor_upgrade_1"),
        new ArmorUpgradeItem("armor_upgrade_2", 5, 2, "armor_upgrade_2"),
        new ArmorUpgradeItem("armor_upgrade_4", 5, 2, "armor_upgrade_4"),
        new ArmorUpgradeItem("armor_upgrade_5", 5, 2, "armor_upgrade_5"),
        new ConsumableItem("grenade_upgrade_1", 20f, 10, "grenade_upgrade_1"),
        new ConsumableItem("grenade_upgrade_2", 20f, 20, "grenade_upgrade_2"),
        new ConsumableItem("temp_dmg_upgrade_1", 10f, 5, "temp_dmg_upgrade_1"),
        new RelicUpgradeItem("hair_upgrade", 100f, 0, "hair_upgrade"),
        new RelicUpgradeItem("luck_upgrade", 0f, 5, "luck_upgrade"),
        new RelicUpgradeItem("sight_upgrade_1", 0f, 5, "sight_upgrade_1"),
        new RelicUpgradeItem("sight_upgrade_2", 0f, 10, "sight_upgrade_2"),
        new RelicUpgradeItem("sight_upgrade_3", 0f, 15, "sight_upgrade_3"),
        new RelicUpgradeItem("speed_upgrade_1", 10f, 0, "speed_upgrade_1"),
        new RelicUpgradeItem("speed_upgrade_2", 10f, 0, "speed_upgrade_2"),
        new WeaponUpgradeItem("barrel_extension", 10, "barrel_extension"),
        new WeaponUpgradeItem("bullet_upgrade_2", 10, "bullet_upgrade_2"),
        new WeaponUpgradeItem("bullet_upgrade_3", 10, "bullet_upgrade_3")
    };
    [SerializeField] private List<RoomDropRate> dropRates = new List<RoomDropRate>{};
    [SerializeField] private List<RoomLootTable> weights = new List<RoomLootTable>{};

    public List<Item> Prototypes()
    {
        return prototypes;
    }

    public List<RoomDropRate> DropRates()
    {
        return dropRates;
    }

/**
 * RollDrop returns a spawnable game object for the dropped item;
 */
    public Item RollDrop(RoomType roomType)
    {
        float dropRate = dropRates.Find(roomRate =>
        {
            return roomRate.GetRoomType() == roomType;
        }).GetDropRate();

        //check if the loot actually drops
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= dropRate)
        {
            RoomLootTable lootTable = weights.Find(roomLootTable =>
            {
                return roomLootTable.GetRoomType() == roomType;
            });

            //Do not continue if the loot table does not exist
            if (lootTable == null)
                return null;

            //fetch total weight for weighted rand drop
            int totalWeight = 0;
            lootTable.GetLootTable().ForEach(itemDropWeight =>
            {
                totalWeight += itemDropWeight.GetDropRate();
            });
            
            int itemRand = UnityEngine.Random.Range(0,totalWeight+1);
            string itemToDropId = null;
            foreach (ItemDropWeight item in lootTable.GetLootTable())
            {
                if (itemRand <= item.GetDropRate())
                {
                    itemToDropId = item.GetSpawnableItem();
                    break;
                } else
                {
                    itemRand -= item.GetDropRate();
                    continue;
                }
            };
            foreach (Item item in prototypes)
            {
                if (item.getItemId() == itemToDropId)
                {
                    return item.Clone();
                }
            }
            return null;
        } else
        {
            return null;
        }
    }


    [Serializable]
    public class RoomDropRate
    {
        [SerializeField]
        private RoomType roomType = RoomType.None;
        [SerializeField]
        private float dropRate = 0.0f;
        
        public RoomType GetRoomType()
        {
            return roomType;
        }
        public float GetDropRate()
        {
            return dropRate;
        }
    }

    [Serializable]
    private class RoomLootTable
    {
        [SerializeField]
        private RoomType roomType;
        [SerializeField]
        private List<ItemDropWeight> lootTable = new List<ItemDropWeight>{};
        
        public RoomType GetRoomType()
        {
            return roomType;
        }
        public List<ItemDropWeight> GetLootTable()
        {
            return lootTable;
        }
    }

    [Serializable]
    private class ItemDropWeight
    {
        [SerializeField]
        private string SpawnableItem;
        [SerializeField]
        private int dropWeight = 0;
        
        public string GetSpawnableItem()
        {
            return SpawnableItem;
        }
        public int GetDropRate()
        {
            return dropWeight;
        }
    }
}
