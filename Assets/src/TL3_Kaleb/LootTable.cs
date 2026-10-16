using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[Serializable]
public class LootTable
{
    private List<Item> prototypes = new List<Item>
    {
        
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
