using UnityEngine;
using TMPro;

public class TL2StressController : MonoBehaviour
{
    [SerializeField] private TMP_Text numberOfItemsViewer;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private GameObject item;

    [SerializeField] private int minimumEnemies = 20;
    [SerializeField] private int targetFPS = 60;

    private int numItems = 0;
    private bool testStopped = false;

    private float elapsedTime = 0f;
    private int frameCount = 0;
    private float averageFPS = 0f;

    void Update()
    {
        // Measure FPS over one second
        elapsedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (elapsedTime >= 1f)
        {
            averageFPS = frameCount / elapsedTime;

            fpsCounter.SetText("FPS: {0:0}", averageFPS);

            // Stop if average FPS drops below target
            if (averageFPS < targetFPS &&
                numItems >= minimumEnemies &&
                !testStopped)
            {
                testStopped = true;

                Debug.Log(
                    "STRESS TEST COMPLETE!\n" +
                    "Total Enemies: " + numItems + "\n" +
                    "Average FPS: " + averageFPS.ToString("F1")
                );
            }

            // Reset FPS measurement
            elapsedTime = 0f;
            frameCount = 0;
        }

        if (testStopped)
            return;

        if (item == null)
            return;

        // Spawn one enemy per frame
        Vector3 spawnPosition = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(-4f, 4f),
            0f
        );

        Instantiate(item, spawnPosition, Quaternion.identity);

        numItems++;

        numberOfItemsViewer.SetText("{0}", numItems);
    }
}