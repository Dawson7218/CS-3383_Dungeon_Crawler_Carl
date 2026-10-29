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
    public override string ApplyEffect(PlayerStats playerStats)
    {
        playerStats.ModifyAttackPower(attackDelta);
         return "consumableItem";
    }
    public void Revert(PlayerStats playerStats)
    {
        if (durationRemaining <= 0) {
            playerStats.ModifyAttackPower(-attackDelta);
        } else
        {
            durationRemaining -= 0.1f;
        }
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
