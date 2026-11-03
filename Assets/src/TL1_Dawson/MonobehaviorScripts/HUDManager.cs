using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] public GameObject hudRoot;
    public GameState gameState;
    HudElement healthBar = new HealthBarElement();
    HudElement gameTimer = new RunTimerElement();
    HudElement pointsCounter = new PointsCounterElement();
    HudElement minimap = new MinimapElement();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //test code will need removed when hooked to other components
        gameState.playerHealth = 100;
        gameState.points = 123;
    }

    // Update is called once per frame
    void Update()
    {
        //temp line 
        gameState.gameTimer += Time.deltaTime;

        healthBar.Refresh(gameState, hudRoot.transform.Find("HealthBar").gameObject);
        pointsCounter.Refresh(gameState, hudRoot.transform.Find("PointsDisplay").gameObject);
        gameTimer.Refresh(gameState, hudRoot.transform.Find("TimeDisplay").gameObject);

        //need to actually get game object data in
    }
}
