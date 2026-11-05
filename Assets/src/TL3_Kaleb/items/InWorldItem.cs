using UnityEngine;

public class InWorldItem : MonoBehaviour
{

    [SerializeField] private Item item;

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null && LootSystem.Instance != null) {
            LootSystem.Instance.ApplyItem(item, playerController.GetStats());
            Destroy(gameObject);
        }
    }

    public Item Item()
    {
        return item;
    }

    internal void SetItem(Item item)
    {
        this.item = item;
    }
}
