using UnityEngine;
using UnityEngine.SceneManagement;

//Draws the standalone shop scene
public class MetaShopUI : MonoBehaviour
{
    //Change the destination in the Inspector if the menu scene is renamed.
    [SerializeField] private string mainMenuScene = "MainMenu";
    //This array controls the order of the three rows. readonly prevents replacing it.
    private readonly StatType[] stats = { StatType.Health, StatType.Defense, StatType.Attack };
    // Cache the manager and the message shown below the upgrade buttons.
    private EconomyManager economy;
    private string message = "";

    private void Start() //Create a manager instance and check if it fails
    {
        economy = EconomyManager.Instance;
        if (economy != null) message = economy.LastError;
    }

    // Unity calls OnGUI for drawing and input events, sometimes several times per frame.
    // GUILayout positions the controls; no Canvas or UI prefab is needed.
    //OnGUI runs once a frame in addition to on GUI inputs
    private void OnGUI()
    {
        //Do not try to read points or levels without a manager in the scene.
        if (economy == null) return;
        //Copy Unity's built-in IMGUI styles and adjust the local copies for readable text.
        GUIStyle text = new GUIStyle(GUI.skin.label) { fontSize = 18 };
        GUIStyle title = new GUIStyle(text) { fontSize = 26, alignment = TextAnchor.MiddleCenter };
        GUIStyle button = new GUIStyle(GUI.skin.button) { fontSize = 18 };
        //Center a 340-pixel-tall panel, with a maximum width of 500 pixels.
        //On narrower windows, leave 10 pixels of space on each side.
        float width = Mathf.Min(500, Screen.width - 20);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - 340) / 2, width, 340), GUI.skin.box);
        GUILayout.Label("Meta Shop", title, GUILayout.Height(42));
        GUILayout.Label("Points: " + economy.GetBalance(), text);
        GUILayout.Space(12);

        //Reuse the same row code once for health, defense, and attack.
        foreach (StatType stat in stats)
        {
            //Place the stat label and its upgrade button next to each other.
            GUILayout.BeginHorizontal();
            GUILayout.Label(stat + "  -  Level " + economy.GetLevel(stat), text, GUILayout.Height(36));
            // Disable the purchase button when points are too low, the stat is capped,
            // or the save could not be loaded safely.
            GUI.enabled = economy.CanPurchase(stat);
            string label = economy.IsMaxLevel(stat) ? "Max level" : "Upgrade (" + economy.GetNextCost(stat) + ")";
            // Button returns true on a click, so drawing the UI does not buy upgrades.
            // The manager handles validation and saving; the UI displays the result.
            if (GUILayout.Button(label, button, GUILayout.Width(170), GUILayout.Height(36)))
                message = economy.TryPurchase(stat) ? stat + " upgraded." : economy.LastError;
            // Enable the remaining controls so Main Menu still works when buying is disabled.
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
        }

        // Reserve a line for purchase/save feedback, followed by the scene's only exit.
        GUILayout.Label(message, text, GUILayout.Height(28));
        if (GUILayout.Button("Main Menu", button, GUILayout.Height(36))) ReturnToMainMenu();
        GUILayout.EndArea();
    }

    public void ReturnToMainMenu()
    {
        // Scene loading requires the target scene to be available in the build.
        if (!Application.CanStreamedLevelBeLoaded(mainMenuScene))
        {
            message = "Main Menu scene is not available.";
            return;
        }
        // Undo any gameplay pause/death time freeze before returning to the menu.
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }
}
