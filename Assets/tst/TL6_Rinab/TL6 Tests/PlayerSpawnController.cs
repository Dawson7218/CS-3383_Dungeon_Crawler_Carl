using UnityEngine;
using TMPro;

public class PlayerSpawnController : MonoBehaviour
{
    [SerializeField] private TMP_Text numberOfPlayersViewer;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private GameObject Player;
    private int numPlayers = 0;
    int currentFPS = 1000;

    

    // Update is called once per frame
    void Update()
    {
        if (currentFPS > 60 || numPlayers < 20) {
            currentFPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            Instantiate(Player);
            ++numPlayers;
            numberOfPlayersViewer.SetText("{0}", numPlayers);
            fpsCounter.SetText("fps: {0}", currentFPS);
        } else
        {
            Debug.Log("fps dropped below 60 to " + currentFPS + " at " + numPlayers + " players spawned  in");
        }
    }
}
