using UnityEngine;
using TMPro;
// Existing UI stress demo: keep spawning text objects while the frame rate permits it.
// This is separate from the meta shop and does not buy or save upgrades.
public class GettyTL4StressTestController : MonoBehaviour
{
    // Assign the two display labels and the prefab to spawn in the Inspector.
    [SerializeField] private TMP_Text numItemsViewer;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private GameObject menuTxtPrefab;
    // Count spawned objects. Start with a high FPS value so the test can begin spawning.
    private int numItems = 0;
    int currentFPS = 1000;

    

    // Unity calls Update once per frame.
    void Update()
    {
        // Spawn at least 20 objects. After that, continue while the last measured FPS is above 60.
        if (currentFPS > 60 || numItems < 20) {
            // Invert the unscaled frame duration to estimate FPS independently of timeScale.
            currentFPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            // Add one object this frame and update both counters on screen.
            Instantiate(menuTxtPrefab);
            ++numItems;
            numItemsViewer.SetText("{0}", numItems); 
            fpsCounter.SetText("fps: {0}", currentFPS);
        } else
        {
            // No more objects are spawned here. FPS is not recalculated in this branch,
            // so once spawning stops, this message is logged on each following frame.
            Debug.Log("fps dropped below 60 to " + currentFPS + " at " + numItems + " items spawned in");
        }
    }
}
