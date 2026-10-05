using UnityEngine;

public class DynBindTest : MonoBehaviour
{
    HudElement healthBar = new HealthBarElement();
    GameState gameState = new GameState()
    {
        tmp = 5
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), healthBar.Refresh(gameState));
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
        {
            healthBar = (healthBar is HealthBarElement) ? new HudElement() : new HealthBarElement();
        }
    }
}
