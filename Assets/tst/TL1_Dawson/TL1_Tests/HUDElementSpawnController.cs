using UnityEngine;
using TMPro;

public class HUDElementSpawnController : MonoBehaviour
{
    [SerializeField] private TMP_Text numberOfElementsViewer;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private GameObject item;
    private int numElements = 0;
    int currentFPS = 1000;
    // Update is called once per frame
    void Update()
    {
        if (currentFPS > 60 || numElements < 20)
        {
            currentFPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            Instantiate(item);
            ++numElements;
            numberOfElementsViewer.SetText("{0}", numElements);
            fpsCounter.SetText("fps: {0}", currentFPS);
        }
        else
        {
            Debug.Log("fps dropped below 60 to " + currentFPS + " at " + numElements + " items spawned in");
        }
    }
}
