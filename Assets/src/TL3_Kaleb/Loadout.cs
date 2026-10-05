using System.Collections.Generic;
using UnityEngine;

public class Loadout
{
    private List<Item> items;
    private List<(string, float)> consumableTimers;

    public List<Item> Items()
    {
        return items;
    }

    public List<(string, float)> ConsumableTimers()
    {
        return consumableTimers;
    }

    public void ApplyItem(Item item/*, PlayerStats stats*/)
    {
        items.Add(item);
        //other functionallity needed here
    }
}
