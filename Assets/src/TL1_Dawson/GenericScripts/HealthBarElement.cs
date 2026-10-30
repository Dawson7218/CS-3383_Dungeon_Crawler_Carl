using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarElement : HudElement
{
    public override bool Refresh(GameState gameState, GameObject hudObject)
    {
        var healthSlider = hudObject.transform.Find("HealthSlider").GetComponent<Slider>();
        var healthNumber = hudObject.transform.Find("HealthNumberDisplay").GetComponent<TextMeshProUGUI>();

        healthSlider.value = gameState.playerHealth / 100;
        healthNumber.text = gameState.playerHealth.ToString();

        return true;
    }
}
