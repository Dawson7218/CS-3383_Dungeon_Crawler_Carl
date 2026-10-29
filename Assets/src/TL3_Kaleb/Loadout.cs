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

    public void ApplyItem(Item item, PlayerStats stats)
    {
        items.Add(item);
        item.ApplyEffect(stats);
        if (item is ConsumableItem consumable)
        {
            consumableTimers.Add((consumable.getConsumableId(), consumable.getDurationRemaining()));
        }
    }

    public ItemMemento CreateMemento()
    {
        return new ItemMemento();
    }

    public void SetMemento(ItemMemento itemMemento)
    {
        
    }
}
