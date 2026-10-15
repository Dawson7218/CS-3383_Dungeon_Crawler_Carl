using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootTable
{
    [SerializeField] private List<Item> prototypes = new List<Item>{};
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

    public Item RollDrop(RoomType roomType)
    {
        switch (roomType)
        {
            default:
                return new RelicUpgradeItem("steak", "steak1", 0.1f, 1);
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
        private GameObject SpawnableItem;
        [SerializeField]
        private int dropWeight = 0;
        
        public GameObject GetRoomType()
        {
            return SpawnableItem;
        }
        public int GetDropRate()
        {
            return dropWeight;
        }
    }
}
