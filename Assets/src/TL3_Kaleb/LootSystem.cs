using System.Reflection.Metadata.Ecma335;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    [SerializeField] private LootTable lootTables;
    private Loadout loadout = new Loadout();
    private ItemMemento heldMemento = new ItemMemento();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ApplyItem(Item item/*, PlayerStats stats*/)
    {
        
    }

    public void GenerateLootAt(RoomType roomType, Vector2 location)
    {
        Item itemToSpawn = lootTables.RollDrop(roomType);
        if (itemToSpawn != null)
        {
            //TODO:
            //Instantiate(objectToSpawn)
        }
    }

    //temporary for continued mvp workage
    public Item RollDrop(RoomType roomType)
    {
        return new RelicUpgradeItem("steak", "steak", 0.1f, 1);
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
