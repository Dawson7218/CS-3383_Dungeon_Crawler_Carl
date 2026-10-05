using System.Collections.Generic;
using UnityEngine;

public class ItemMemento
{
    private List<Item> items;
    private List<(string, float)> consumableTimers;
    //private PlayerStats statBaseline;
    private int schemaVersion;

    protected void CopyFrom(Loadout loadout)
    {
        items = loadout.Items();
        consumableTimers = loadout.ConsumableTimers();
    }

    protected void RestoreInto(Loadout loadout)
    {
        //loadout restore functionallity
    }
}
