using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarElement : HudElement
{
    public override bool Refresh(GameState gameState, GameObject hudObject)
    {
        try
        {
            var healthSlider = hudObject.transform.Find("HealthSlider").GetComponent<Slider>();
            var healthNumber = hudObject.transform.Find("HealthNumberDisplay").GetComponent<TextMeshProUGUI>();

            healthSlider.value = (float)gameState.playerHealth / gameState.maxHealth * 100f;
            healthNumber.text = gameState.playerHealth.ToString();

            return true;
        }
        catch
        {
            return false;
        }
    }
}
