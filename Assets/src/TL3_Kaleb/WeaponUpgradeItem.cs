using UnityEngine;

public class WeaponUpgradeItem : Item
{
    private int attackDelta = 0;

    protected WeaponUpgradeItem(string itemId, ItemKind itemKind, int attackDelta) : base(itemId, itemKind)
    {
        this.attackDelta = attackDelta;
    }
    public override string ApplyEffect(/*will accept player stats*/)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "weaponUpgradeItem";
    }

    public override Item Clone()
    {
        return new WeaponUpgradeItem(getItemId(), getItemKind(), attackDelta);
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
