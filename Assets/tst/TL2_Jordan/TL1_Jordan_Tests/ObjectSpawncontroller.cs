using UnityEngine;
using TMPro;

public class ObjectSpawncontroller : MonoBehaviour
{
    [SerializeField] private TMP_Text numberOfObjectsUpsViewer;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private GameObject roomPrefab;
    private int numItems = 0;
    int currentFPS = 1000;

    

    // Update is called once per frame
    void Update()
    {
        if (currentFPS > 60 || numItems < 20) {
            currentFPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            Instantiate(roomPrefab);
            ++numItems;
            numberOfObjectsUpsViewer.SetText("{0}", numItems);
            fpsCounter.SetText("fps: {0}", currentFPS);
        }
    }
}
