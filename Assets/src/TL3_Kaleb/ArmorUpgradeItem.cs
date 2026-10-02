using UnityEngine;

public class ArmorUpgradeItem : Item
{
    private int defenseDelta = 0;
    private int maxHealthDelta = 0;

    protected ArmorUpgradeItem(string itemId, ItemKind itemKind, int defenseDelta, int maxHealthDelta) : base(itemId, itemKind)
    {
        this.defenseDelta = defenseDelta;
        this.maxHealthDelta = maxHealthDelta;
    }
    public override string ApplyEffect(/*will accept player stats*/)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "applyEffectItem";
    }

    public override Item Clone()
    {
        return new ArmorUpgradeItem(getItemId(), getItemKind(), defenseDelta, maxHealthDelta);
    }

    public int getDefenseDelta()
    {
        return defenseDelta;
    }

    public void setDefenseDelta(int defenseDelta)
    {
        this.defenseDelta = defenseDelta;
    }

    public int getMaxHealthDelta()
    {
        return maxHealthDelta;
    }

    public void setMaxHealthDelta(int maxHealthDelta)
    {
        this.maxHealthDelta = maxHealthDelta;
    }
}
