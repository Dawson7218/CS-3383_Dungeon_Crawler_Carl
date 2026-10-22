using UnityEngine;
using TMPro;
public class GettyTL4StressTestController : MonoBehaviour
{
    [SerializeField] private TMP_Text numItemsViewer;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private GameObject menuTxtPrefab;
    private int numItems = 0;
    int currentFPS = 1000;

    

    // Update is called once per frame
    void Update()
    {
        if (currentFPS > 60 || numItems < 20) {
            currentFPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            Instantiate(menuTxtPrefab);
            ++numItems;
            numItemsViewer.SetText("{0}", numItems); 
            fpsCounter.SetText("fps: {0}", currentFPS);
        } else
        {
            Debug.Log("fps dropped below 60 to " + currentFPS + " at " + numItems + " items spawned in");
        }
    }
}