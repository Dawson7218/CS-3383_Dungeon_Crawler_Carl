using System;
using UnityEngine;

[Serializable]
public class Item // YOUR superclass name​
{
    [SerializeField] private string itemId = "none";
    [SerializeField] private ItemKind itemKind = ItemKind.None;
    [SerializeField] private string texturePath = "";

    public Item(string itemId, ItemKind itemKind) : this(itemId, itemKind, "hair_upgrade")
    {}

    public Item(string itemId, ItemKind itemKind, string texturePath)
    {
        this.itemId = itemId;
        this.itemKind = itemKind;
        this.texturePath = texturePath;
    }

    public Item() {}

    public virtual string ApplyEffect(PlayerStats playerStats)
    {
        playerStats.ModifyMaxHP(1000000);
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

    public string getTexture()
    {
        return texturePath;
    }
}
