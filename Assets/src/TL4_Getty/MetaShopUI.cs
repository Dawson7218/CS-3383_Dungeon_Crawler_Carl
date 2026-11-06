using UnityEngine;
using UnityEngine.SceneManagement;

// Runtime labels and buttons sit over the orbital console skin.
public class MetaShopUI : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private Font pixelFont;

    private readonly StatType[] stats = { StatType.Health, StatType.Defense, StatType.Attack };
    private static readonly Color Ice = new Color32(193, 220, 238, 255);
    private static readonly Color Muted = new Color32(145, 150, 176, 255);
    private static readonly Color Cream = new Color32(239, 211, 163, 255);
    private EconomyManager economy;
    private Texture2D skin;
    private Texture2D[] icons;
    private string message = "";
    private GUIStyle title, category, subtitle, level, button, points, unavailableButton, navigationButton;
    private int lastPurchased = -1;
    private float purchaseTime;

    private void Start()
    {
        economy = EconomyManager.Instance;
        if (economy != null) message = economy.LastError;
        skin = Resources.Load<Texture2D>("TL4/OrbitalShopSkin");
        icons = new[]
        {
            Resources.Load<Texture2D>("TL4/Minecraft/heart"),
            Resources.Load<Texture2D>("TL4/Minecraft/armor"),
            Resources.Load<Texture2D>("TL4/Minecraft/sword")
        };
        if (skin == null) Debug.LogError("Missing orbital meta shop skin.", this);
        for (int i = 0; i < icons.Length; i++)
            if (icons[i] == null) Debug.LogError("Missing TL4 shop icon for " + stats[i], this);
    }

    private void OnGUI()
    {
        if (title == null) CreateStyles();
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        bool previousEnabled = GUI.enabled;
        try
        {
            // Keep the complete artwork visible at any display aspect ratio.
            GUI.matrix = Matrix4x4.identity;
            GUI.color = Color.white;
            Fill(new Rect(0, 0, Screen.width, Screen.height), new Color32(6, 10, 28, 255));
            float scale = Mathf.Min(Screen.width / 560f, Screen.height / 315f);
            if (scale <= 0) return;
            GUI.matrix = Matrix4x4.TRS(new Vector3(
                Mathf.Round((Screen.width - 560 * scale) / 2),
                Mathf.Round((Screen.height - 315 * scale) / 2), 0), Quaternion.identity, Vector3.one * scale);
            if (skin != null) GUI.DrawTexture(new Rect(0, 0, 560, 315), skin, ScaleMode.StretchToFill);

            GUI.Label(new Rect(351, 99, 92, 18),
                economy != null ? "POINTS: " + economy.GetBalance() : "OFFLINE", points);
            for (int i = 0; i < stats.Length; i++) DrawUpgrade(i);

            GUI.Label(new Rect(214, 257, 98, 20), "META SHOP", title);
            if (!string.IsNullOrEmpty(message))
            {
                Fill(new Rect(166, 244, 246, 12), new Color32(9, 12, 28, 240));
                GUI.Label(new Rect(168, 243, 242, 14), message, subtitle);
            }
            if (ConsoleButton(new Rect(435, 259, 81, 17), "MAIN MENU", true, true)) ReturnToMainMenu();
        }
        finally
        {
            GUI.matrix = previousMatrix;
            GUI.color = previousColor;
            GUI.enabled = previousEnabled;
        }
    }

    private void DrawUpgrade(int index)
    {
        StatType stat = stats[index];
        int y = 138 + index * 36;
        if (icons != null && icons[index] != null)
        {
            int size = stat == StatType.Health ? 27 : 24;
            GUI.DrawTexture(new Rect(131 - size / 2, y + (28 - size) / 2, size, size),
                icons[index], ScaleMode.ScaleToFit, true);
        }
        GUI.Label(new Rect(171, y + 5, 106, 20), stat.ToString().ToUpperInvariant(), category);
        GUI.Label(new Rect(299, y + 5, 65, 20),
            "LEVEL " + (economy != null ? economy.GetLevel(stat) : 0), level);

        bool maxed = economy != null && economy.IsMaxLevel(stat);
        bool available = economy != null && economy.CanPurchase(stat);
        string label = economy == null ? "OFFLINE" : maxed ? "MAX LEVEL" : "UPGRADE " + economy.GetNextCost(stat);
        Rect purchase = new Rect(382, y + 5, 82, 18);
        if (lastPurchased == index && Time.unscaledTime - purchaseTime < .4f)
            Fill(new Rect(169, y - 1, 198, 31), new Color32(167, 107, 40, 35));
        if (ConsoleButton(purchase, label, available))
        {
            if (economy.TryPurchase(stat))
            {
                message = stat.ToString().ToUpperInvariant() + " UPGRADED";
                lastPurchased = index;
                purchaseTime = Time.unscaledTime;
            }
            else message = economy.LastError;
        }
    }

    private bool ConsoleButton(Rect rect, string label, bool available, bool navigation = false)
    {
        bool enabled = GUI.enabled;
        if (!available)
        {
            Fill(rect, new Color32(10, 8, 18, 160));
            GUI.Label(rect, label, unavailableButton);
            return false;
        }
        else if (rect.Contains(Event.current.mousePosition))
            Fill(rect, new Color32(244, 190, 100, 40));
        GUI.enabled = enabled && available;
        bool clicked = GUI.Button(rect, label, navigation ? navigationButton : button);
        GUI.enabled = enabled;
        return clicked;
    }

    private GUIStyle LabelStyle(int size, Color color, TextAnchor alignment)
    {
        GUIStyle style = new GUIStyle { font = pixelFont, fontSize = size, alignment = alignment, wordWrap = false };
        style.normal.textColor = color;
        return style;
    }

    private void CreateStyles()
    {
        title = LabelStyle(18, Ice, TextAnchor.MiddleCenter);
        category = LabelStyle(22, Ice, TextAnchor.MiddleLeft);
        subtitle = LabelStyle(14, Muted, TextAnchor.MiddleLeft);
        level = LabelStyle(20, Ice, TextAnchor.MiddleCenter);
        points = LabelStyle(18, Cream, TextAnchor.MiddleCenter);
        points.contentOffset = new Vector2(0, -4);
        title.contentOffset = new Vector2(0, -1);
        button = LabelStyle(12, new Color32(22, 13, 10, 255), TextAnchor.MiddleCenter);
        unavailableButton = LabelStyle(12, Cream, TextAnchor.MiddleCenter);
        navigationButton = LabelStyle(16, Cream, TextAnchor.MiddleCenter);
        navigationButton.hover.textColor = Color.white;
        navigationButton.active.textColor = Ice;
        navigationButton.focused.textColor = Color.white;
        button.hover.textColor = new Color32(22, 13, 10, 255);
        button.active.textColor = Cream;
        button.focused.textColor = Cream;
        if (pixelFont != null && pixelFont.material.mainTexture != null)
            pixelFont.material.mainTexture.filterMode = FilterMode.Point;
    }

    private static void Fill(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previous;
    }

    public void ReturnToMainMenu()
    {
        if (!Application.CanStreamedLevelBeLoaded(mainMenuScene))
        {
            message = "Main Menu scene is not available.";
            return;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }
}
