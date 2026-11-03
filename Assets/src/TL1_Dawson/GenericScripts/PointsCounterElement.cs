using TMPro;
using UnityEngine;

public class PointsCounterElement : HudElement
{
    public override bool Refresh(GameState gameState, GameObject hudObject)
    {
        try
        {
            var healthNumber = hudObject.transform.Find("PointsCounter").GetComponent<TextMeshProUGUI>();
            healthNumber.text = gameState.points.ToString();

            return true;
        }
        catch { return false; }
    }
}
