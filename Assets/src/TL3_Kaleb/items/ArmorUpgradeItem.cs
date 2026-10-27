using UnityEngine;

public class ArmorUpgradeItem : Item
{
    private int defenseDelta = 0;
    private int maxHealthDelta = 0;

    public ArmorUpgradeItem(string itemId, int defenseDelta, int maxHealthDelta, string texturePath) : base(itemId, ItemKind.ArmorUpgrade, texturePath)
    {
        this.defenseDelta = defenseDelta;
        this.maxHealthDelta = maxHealthDelta;
    }
    public override string ApplyEffect(PlayerStats playerStats)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "applyEffectItem";
    }

    public override Item Clone()
    {
        return new ArmorUpgradeItem(getItemId(), defenseDelta, maxHealthDelta, getTexture());
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
