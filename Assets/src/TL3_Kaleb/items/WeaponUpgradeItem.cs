using UnityEngine;

public class WeaponUpgradeItem : Item
{
    private int attackDelta = 0;

    public WeaponUpgradeItem(string itemId, int attackDelta, string texturePath) : base(itemId, ItemKind.WeaponUpgrade, texturePath)
    {
        this.attackDelta = attackDelta;
    }
    public override string ApplyEffect(PlayerStats playerStats)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "weaponUpgradeItem";
    }

    public override Item Clone()
    {
        return new WeaponUpgradeItem(getItemId(), attackDelta, getTexture());
    }

    public int getAttackDelta()
    {
        return attackDelta;
    }

    public void setAttackDelta(int attackDelta)
    {
        this.attackDelta = attackDelta;
    }
}
