using UnityEngine;

public class HUDManager : MonoBehaviour
{
    HudElement healthBar = new HealthBarElement();
    HudElement gameTimer = new RunTimerElement();
    HudElement pointsCounter = new PointsCounterElement();
    HudElement minimap = new MinimapElement();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
