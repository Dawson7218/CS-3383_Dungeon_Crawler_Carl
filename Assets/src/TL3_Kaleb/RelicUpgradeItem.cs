using UnityEngine;

public class RelicUpgradeItem : Item
{
    private string relicId = "nothing";
    private float moveSpeedDelta = 0;
    private int critChanceDelta = 0;

    public RelicUpgradeItem(string itemId, string relicId, float moveSpeedDelta, int critChanceDelta) : base(itemId, ItemKind.Relic)
    {
        this.relicId = relicId;
        this.moveSpeedDelta = moveSpeedDelta;
        this.critChanceDelta = critChanceDelta;
    }
    public override string ApplyEffect(/*will accept player stats*/)
    {
        /* Once the player stats structure has been created this will
         * update/change the inputed player stats
         */
         return "relicItem";
    }

    public override Item Clone()
    {
        return new RelicUpgradeItem(getItemId(), relicId, moveSpeedDelta, critChanceDelta);
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
