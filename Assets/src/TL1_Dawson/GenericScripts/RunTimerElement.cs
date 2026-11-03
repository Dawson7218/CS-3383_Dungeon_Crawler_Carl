using System;
using TMPro;
using UnityEngine;

public class RunTimerElement : HudElement
{
    public override bool Refresh(GameState gameState, GameObject hudObject)
    {
        try
        {
            var timer = hudObject.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            TimeSpan span = TimeSpan.FromSeconds(gameState.gameTimer);
            timer.text = span.ToString(@"mm\:ss");
            return true;
        }
        catch
        {
            return false;
        }
        
    }
}
