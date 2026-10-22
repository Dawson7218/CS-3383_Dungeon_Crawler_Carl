using UnityEngine;

public class LootSystem : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefabBase;
    [SerializeField] private LootTable lootTables;
    private Loadout loadout = new Loadout();
    private ItemMemento heldMemento = new ItemMemento();

    //Code gets simpler if I use a singleton pattern for this class
    public static LootSystem Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        // Assign this instance as the global singleton
        Instance = this;
    }

    public void ApplyItem(Item item/*, PlayerStats stats*/)
    {
        
    }

    public void GenerateLootAt(RoomType roomType, Vector2 location)
    {
        Item itemToSpawn = lootTables.RollDrop(roomType);
        if (itemToSpawn != null)
        {
            Vector3 spawnPosition = new Vector3(location.x, location.y, 0f);

            // Instantiate the item base prefab at the specific position with no rotation
            GameObject itemspawned = Instantiate(itemPrefabBase, spawnPosition, Quaternion.identity);
            // Assign the kind of item it is
            itemspawned.GetComponent<InWorldItem>().SetItem(itemToSpawn);
            itemspawned.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("TL3_Kaleb/" + itemToSpawn.getTexture());
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
