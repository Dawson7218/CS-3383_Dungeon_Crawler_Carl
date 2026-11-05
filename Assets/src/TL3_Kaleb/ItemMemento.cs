using System.Collections.Generic;

// Opaque checkpoint of a loadout. LootSystem may hold this reference, but only Loadout may read or write it.
public class ItemMemento
{
    private List<string> equippedItemIds = new List<string>();
    private List<(string, float)> consumableTimers = new List<(string, float)>();
    private PlayerStats statBaseline;
    private readonly int schemaVersion = 1;

    internal ItemMemento()
    {
    }

    internal void CopyFrom(Loadout loadout)
    {
        equippedItemIds = new List<string>();
        consumableTimers = new List<(string, float)>();
        statBaseline = null;

        if (loadout == null)
            return;

        List<Item> items = loadout.Items();
        if (items != null)
        {
            foreach (Item item in items)
            {
                if (item != null)
                    equippedItemIds.Add(item.getItemId());
            }
        }

        List<(string, float)> timers = loadout.ConsumableTimers();
        if (timers != null)
        {
            foreach ((string, float) timer in timers)
                consumableTimers.Add((timer.Item1, timer.Item2));
        }

        statBaseline = CopyStats(loadout.BoundStats());
    }

    internal void RestoreInto(Loadout loadout)
    {
        if (loadout == null || schemaVersion != 1)
            return;

        loadout.ReplaceFromSnapshot(equippedItemIds, consumableTimers, statBaseline);
    }

    private static PlayerStats CopyStats(PlayerStats stats)
    {
        if (stats == null)
            return null;

        PlayerStats copy = new PlayerStats(
            stats.maxHP,
            stats.moveSpeed,
            stats.attackPower,
            stats.critChance,
            stats.critMulti,
            stats.defense);
        copy.currentHP = stats.currentHP;
        return copy;
    }
}
