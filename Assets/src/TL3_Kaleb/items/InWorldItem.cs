using System;
using UnityEngine;

public class InWorldItem : MonoBehaviour
{

    [SerializeField] private Item item;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LootSystem.Instance.ApplyItem(item);
        }
        Destroy(gameObject);
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
