using System.Reflection.Metadata.Ecma335;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    private LootTable lootTables = new LootTable();
    private Loadout loadout;
    private ItemMemento heldMemento;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ApplyItem(Item item/*, PlayerStats stats*/)
    {
        
    }

    public Item RollDrop(RoomType roomType)
    {
        return lootTables.RollDrop(roomType);
    }

    public ItemMemento SaveAState()
    {
        return heldMemento;
    }

    public void RestoreState(ItemMemento itemMemento)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LootTable Table()
    {
        return lootTables;
    }
    public Loadout Loadout()
    {
        return loadout;
    }
}
