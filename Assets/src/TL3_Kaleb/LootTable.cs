using System.Collections.Generic;
using UnityEngine;

public class LootTable
{
    private List<Item> prototypes = new List<Item>{};
    private Dictionary<RoomType, float> dropRates = new Dictionary<RoomType, float>{};
    private Dictionary<RoomType, (string, int)> weights = new Dictionary<RoomType, (string, int)>{};

    public List<Item> Prototypes()
    {
        return prototypes;
    }

    public Dictionary<RoomType, float> DropRates()
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
}
