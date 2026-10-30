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

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.Refresh(gameState, hudRoot.transform.Find("HealthBar").gameObject);
        //need to actually get game object data in
    }
}
