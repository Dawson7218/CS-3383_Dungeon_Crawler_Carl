using System;
using UnityEngine;

[Serializable]
public class Item // YOUR superclass name​
{
    [SerializeField] private string itemId = "none";
    [SerializeField] private ItemKind itemKind = ItemKind.None;
    [SerializeField] private string texturePath = "";

    public Item(string itemId, ItemKind itemKind)
    {
        this.itemId = itemId;
        this.itemKind = itemKind;
    }

    public Item() {}

    public virtual string ApplyEffect(/*will accept player stats*/)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "basicItem";
    }

    public virtual Item Clone()
    {
        return new Item(itemId, itemKind);
    }

    public ItemKind getItemKind()
    {
        return itemKind;
    }

    public string getItemId()
    {
        return itemId;
    }
}
