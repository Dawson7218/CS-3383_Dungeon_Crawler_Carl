using System;
using UnityEngine;

public class Item // YOUR superclass name​
{
    private string itemId = "none";
    private ItemKind itemKind = ItemKind.None;

    public Item(string itemId, ItemKind itemKind)
    {
        this.itemId = itemId;
        this.itemKind = itemKind;
    }

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

    public void setItemKind(ItemKind itemKind)
    {
        this.itemKind = itemKind;
    }

    public string getItemId()
    {
        throw new NotImplementedException();
    }

    public void setItemId(string itemId)
    {
        this.itemId = itemId;
    }
}
