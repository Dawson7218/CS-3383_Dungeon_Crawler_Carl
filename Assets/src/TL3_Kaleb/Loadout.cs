using System.Collections.Generic;

// Originator. Creates a checkpoint of the equipped items and writes one back when asked.
public class Loadout
{
    private List<Item> items = new List<Item>();
    private List<(string, float)> consumableTimers = new List<(string, float)>();
    private PlayerStats boundStats;
    private LootTable table;

    public List<Item> Items()
    {
        return items;
    }

    public List<(string, float)> ConsumableTimers()
    {
        return consumableTimers;
    }

    public void BindLootTable(LootTable lootTable)
    {
        if (lootTable != null)
            table = lootTable;
    }

    public void ApplyItem(Item item, PlayerStats stats)
    {
        if (item == null)
            return;

        items.Add(item);
        if (stats != null)
        {
            item.ApplyEffect(stats);
            boundStats = stats;
        }

        if (item is ConsumableItem consumable)
        {
            consumableTimers.Add((consumable.getConsumableId(), consumable.getDurationRemaining()));
        }
    }

    public ItemMemento CreateMemento()
    {
        ItemMemento memento = new ItemMemento();
        memento.CopyFrom(this);
        return memento;
    }

    public void SetMemento(ItemMemento itemMemento)
    {
        if (itemMemento == null)
            return;

        itemMemento.RestoreInto(this);
    }

    internal PlayerStats BoundStats()
    {
        return boundStats;
    }

    internal void ReplaceFromSnapshot(
        List<string> equippedItemIds,
        List<(string, float)> timers,
        PlayerStats baseline)
    {
        items = new List<Item>();
        int timerIndex = 0;

        if (equippedItemIds != null)
        {
            foreach (string itemId in equippedItemIds)
            {
                Item prototype = FindPrototype(itemId);
                if (prototype == null)
                    continue;

                Item clone = prototype.Clone();
                if (clone is ConsumableItem consumable
                    && timers != null
                    && timerIndex < timers.Count)
                {
                    consumable.setDurationRemaining(timers[timerIndex].Item2);
                    timerIndex++;
                }

                items.Add(clone);
            }
        }

        consumableTimers = new List<(string, float)>();
        foreach (Item item in items)
        {
            if (item is ConsumableItem consumable)
            {
                consumableTimers.Add((consumable.getConsumableId(), consumable.getDurationRemaining()));
            }
        }

        WriteStats(baseline, boundStats);
    }

    private Item FindPrototype(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        LootTable source = table ?? new LootTable();
        List<Item> prototypes = source.Prototypes();
        if (prototypes == null)
            return null;

        foreach (Item prototype in prototypes)
        {
            if (prototype != null && prototype.getItemId() == itemId)
                return prototype;
        }

        return null;
    }

    private static void WriteStats(PlayerStats source, PlayerStats target)
    {
        if (source == null || target == null)
            return;

        target.maxHP = source.maxHP;
        target.currentHP = source.currentHP;
        target.moveSpeed = source.moveSpeed;
        target.attackPower = source.attackPower;
        target.defense = source.defense;
        target.critChance = source.critChance;
        target.critMulti = source.critMulti;
    }
}
