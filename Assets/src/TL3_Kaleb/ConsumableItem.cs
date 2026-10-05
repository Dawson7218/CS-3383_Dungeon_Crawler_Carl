using UnityEditor.VersionControl;
using UnityEngine;

public class ConsumableItem : Item
{
    private string consumableId = "nothing";
    private float durationRemaining = 0;
    private int attackDelta = 0;

    protected ConsumableItem(string itemId, ItemKind itemKind, string consumableId, float durationRemaining, int attackDelta) : base(itemId, itemKind)
    {
        this.consumableId = consumableId;
        this.durationRemaining = durationRemaining;
        this.attackDelta = attackDelta;
    }
    public override string ApplyEffect(/*will accept player stats*/)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "consumableItem";
    }
    public void Revert(/*will accept player stats*/)
    {
        /* Once the player stats structure has been created this will
         * revert the changes made to the inputed player stats by
         * ApplyEfect
         */
    }

    public override Item Clone()
    {
        return new ConsumableItem(getItemId(), getItemKind(), consumableId, durationRemaining, attackDelta);
    }

    public string getConsumableId()
    {
        return consumableId;
    }

    public void setConsumableId(string consumableId)
    {
        this.consumableId = consumableId;
    }

    public float getDurationRemaining()
    {
        return durationRemaining;
    }

    public void setDurationRemaining(float durationRemaining)
    {
        this.durationRemaining = durationRemaining;
    }
}
