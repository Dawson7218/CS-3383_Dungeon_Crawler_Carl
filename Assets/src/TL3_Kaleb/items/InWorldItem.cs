using UnityEngine;

public class InWorldItem : MonoBehaviour
{

    [SerializeField] private Item item;
    [SerializeField] private LootSystem lootSystem;

    public InWorldItem(LootSystem lootSystem)
    {
        this.lootSystem = lootSystem;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            lootSystem.ApplyItem(item);
        }
    }

    public Item Item()
    {
        return item;
    }
}
