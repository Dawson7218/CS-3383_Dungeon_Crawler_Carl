using UnityEditor.VersionControl;
using UnityEngine;

public class ConsumableItem : Item
{
    static private int consumables = 0;
    private string consumableId = "nothing";
    private float durationRemaining = 0;
    private int attackDelta = 0;

    public ConsumableItem(string itemId, float durationRemaining, int attackDelta, string texturePath) : base(itemId, ItemKind.Consumable, texturePath)
    {
        this.consumableId = itemId + "_" + consumables++;
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
        return new ConsumableItem(getItemId(), durationRemaining, attackDelta, getTexture());
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
