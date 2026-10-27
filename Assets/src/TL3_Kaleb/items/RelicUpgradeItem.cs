using UnityEngine;

public class RelicUpgradeItem : Item
{
    static private int relics = 0;
    private string relicId = "nothing";
    private float moveSpeedDelta = 0;
    private int critChanceDelta = 0;

    public RelicUpgradeItem(string itemId, float moveSpeedDelta, int critChanceDelta, string texturePath) : base(itemId, ItemKind.Relic, texturePath)
    {
        this.relicId = itemId + "_" + relics++;
        this.moveSpeedDelta = moveSpeedDelta;
        this.critChanceDelta = critChanceDelta;
    }
    public override string ApplyEffect(PlayerStats playerStats)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "relicItem";
    }

    public override Item Clone()
    {
        return new RelicUpgradeItem(getItemId(), moveSpeedDelta, critChanceDelta, getTexture());
    }

    public string getRelicId()
    {
        return relicId;
    }

    public void setRelicId(string relicId)
    {
        this.relicId = relicId;
    }

    public float getmoveSpeedDelta()
    {
        return moveSpeedDelta;
    }

    public void setmoveSpeedDelta(float moveSpeedDelta)
    {
        this.moveSpeedDelta = moveSpeedDelta;
    }
}
