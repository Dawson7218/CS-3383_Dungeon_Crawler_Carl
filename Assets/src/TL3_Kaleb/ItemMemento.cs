using System.Collections.Generic;
using UnityEngine;

public class ItemMemento
{
    private List<string> equipedItemIds;
    private List<(string, float)> consumableTimers;
    //private PlayerStats statBaseline;
    private int schemaVersion = 1;

    protected void CopyFrom(Loadout loadout)
    {
        loadout.Items().ForEach( item => {
            equipedItemIds.Add(item.getItemId());
        });
        consumableTimers = loadout.ConsumableTimers();
    }

    protected void RestoreInto(Loadout loadout)
    {
        //loadout restore functionallity
    }
}
