using UnityEngine;
using UnityEngine.SceneManagement;

// The shop still uses the original economy. This script only draws its terminal skin.
public class MetaShopUI : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private Font pixelFont;

    private readonly StatType[] stats = { StatType.Health, StatType.Defense, StatType.Attack };
    private static readonly Color Green = new Color32(111, 235, 158, 255);
    private static readonly Color DimGreen = new Color32(53, 116, 78, 255);
    private static readonly Color ScreenColor = new Color32(3, 19, 13, 255);
    private EconomyManager economy;
    private string message = "";
    private GUIStyle text, title, button;
    private Texture2D[] icons;

    private void Start()
    {
        // The manager has already loaded the player's points and upgrades in Awake.
        economy = EconomyManager.Instance;
        if (economy != null) message = economy.LastError;
        icons = new[]
        {
            MakeIcon(new[] { ".##.##..", "#++#xx#.", "#++xxx#.", "#xxxxx#.", ".#xxx#..", "..#x#...", "...#....", "........" }, new Color32(226, 53, 76, 255)),
            MakeIcon(new[] { "..##....", ".#++##..", "#++xxx#.", "#xxxxx#.", ".#xxx#..", ".#xxx#..", "..#x#...", "...#...." }, new Color32(49, 152, 221, 255)),
            MakeIcon(new[] { "........", ".######.", "#++xxxoo", "########", "..#xx#..", ".#xx#...", ".###....", "........" }, new Color32(135, 147, 158, 255))
        };
    }

    // Draw in a small 400 x 240 space. Whole-number scaling keeps the pixels crisp.
    private void OnGUI()
    {
        if (text == null) CreateStyles();
        Matrix4x4 oldMatrix = GUI.matrix;
        Color oldColor = GUI.color;
        bool oldEnabled = GUI.enabled;
        GUI.color = Color.white;
        float fit = Mathf.Min(Screen.width / 400f, Screen.height / 240f);
        float scale = fit >= 1 ? Mathf.Floor(fit) : fit;
        GUI.matrix = Matrix4x4.TRS(new Vector3(
            Mathf.Round((Screen.width - 400 * scale) / 2),
            Mathf.Round((Screen.height - 240 * scale) / 2), 0), Quaternion.identity, Vector3.one * scale);

        // Cover the whole display, including the margins around the centered terminal.
        Fill(new Rect(-Screen.width / scale, -Screen.height / scale,
            Screen.width * 3 / scale, Screen.height * 3 / scale), new Color32(4, 9, 16, 255));
        DrawStars();
        DrawMonitor();

        GUI.Label(new Rect(42, 33, 316, 27), "META SHOP", title);
        GUI.Label(new Rect(42, 63, 316, 16), economy != null ? "POINTS: " + economy.GetBalance() : "ECONOMY UNAVAILABLE", button);
        for (int i = 0; i < stats.Length; i++) DrawUpgrade(stats[i], i);

        // Long save/load errors wrap inside the monitor; normal purchases need one line.
        GUI.Label(new Rect(48, 183, 304, 14), message, text);
        if (TerminalButton(new Rect(140, 198, 120, 18), "MAIN MENU", true)) ReturnToMainMenu();

        // Other OnGUI scripts should receive their original drawing state.
        GUI.matrix = oldMatrix;
        GUI.color = oldColor;
        GUI.enabled = oldEnabled;
    }

    private void DrawUpgrade(StatType stat, int index)
    {
        int y = 85 + index * 33;
        Frame(new Rect(45, y, 310, 28), DimGreen, ScreenColor);
        if (icons != null) GUI.DrawTexture(new Rect(59, y + 6, 16, 16), icons[index]);
        GUI.Label(new Rect(85, y + 5, 90, 18), stat.ToString().ToUpperInvariant(), text);
        GUI.Label(new Rect(178, y + 5, 63, 18), "Level " + (economy != null ? economy.GetLevel(stat) : 0), text);

        bool maxed = economy != null && economy.IsMaxLevel(stat);
        string label = maxed ? "MAX LEVEL" : "UPGRADE " + (economy != null ? economy.GetNextCost(stat) : 0);
        bool available = economy != null && economy.CanPurchase(stat);
        if (TerminalButton(new Rect(249, y + 4, 97, 20), label, available))
            message = economy.TryPurchase(stat) ? stat + " upgraded." : economy.LastError;
    }

    private bool TerminalButton(Rect rect, string label, bool available)
    {
        // Hover/focus and press states make the plain pixel buttons feel interactive.
        bool hovered = available && rect.Contains(Event.current.mousePosition);
        Color edge = available ? Green : DimGreen;
        Frame(rect, edge, hovered ? new Color32(20, 62, 37, 255) : ScreenColor);
        bool previousEnabled = GUI.enabled;
        GUI.enabled = available && previousEnabled;
        bool clicked = GUI.Button(rect, label, button);
        GUI.enabled = previousEnabled;
        return clicked;
    }

    private void DrawMonitor()
    {
        // Layered flat rectangles form a stepped metal casing, not a generated image.
        Fill(new Rect(24, 18, 352, 206), Color.black);
        Fill(new Rect(22, 18, 356, 202), new Color32(28, 36, 40, 255));
        Fill(new Rect(24, 16, 352, 206), new Color32(65, 77, 83, 255));
        Fill(new Rect(28, 20, 344, 198), new Color32(38, 49, 52, 255));
        Frame(new Rect(32, 24, 336, 194), DimGreen, ScreenColor);
        Fill(new Rect(350, 18, 5, 3), new Color32(239, 180, 69, 255));
        Fill(new Rect(52, 46, 46, 1), Green);
        Fill(new Rect(302, 46, 46, 1), Green);
    }

    private void DrawStars()
    {
        // Fixed arithmetic gives a repeatable starfield without changing Unity's random seed.
        for (int i = 0; i < 42; i++)
        {
            int x = (i * 73 + 9) % 400;
            int y = (i * 47 + 5) % 240;
            Fill(new Rect(x, y, 1, 1), i % 3 == 0 ? new Color32(180, 209, 225, 255) : new Color32(63, 99, 123, 255));
            if (i % 11 == 0)
            {
                Fill(new Rect(x - 1, y, 3, 1), new Color32(126, 172, 202, 255));
                Fill(new Rect(x, y - 1, 1, 3), new Color32(126, 172, 202, 255));
            }
        }
    }

    private void CreateStyles()
    {
        // Cache styles once instead of allocating new ones on every repaint.
        text = new GUIStyle { font = pixelFont, fontSize = 16, alignment = TextAnchor.MiddleLeft, wordWrap = true };
        text.normal.textColor = Green;
        title = new GUIStyle(text) { fontSize = 32, alignment = TextAnchor.MiddleCenter, wordWrap = false };
        button = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter, wordWrap = false };
        button.hover.textColor = new Color32(191, 255, 209, 255);
        button.active.textColor = Color.white;
        button.focused.textColor = Color.white;
        button.onNormal.textColor = Green;
        if (pixelFont != null && pixelFont.material.mainTexture != null)
            pixelFont.material.mainTexture.filterMode = FilterMode.Point;
    }

    private static void Fill(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }

    private static void Frame(Rect rect, Color border, Color inside)
    {
        Fill(rect, border);
        Fill(new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2), inside);
    }

    private static Texture2D MakeIcon(string[] rows, Color color)
    {
        // Each character is one pixel: outline, base color, highlight, or gun muzzle.
        Texture2D icon = new Texture2D(8, 8, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                char pixel = rows[y][x];
                Color ink = pixel == '#' ? new Color32(1, 7, 7, 255) : pixel == 'x' ? color :
                    pixel == '+' ? Color.Lerp(color, Color.white, .55f) : pixel == 'o' ? new Color32(245, 177, 53, 255) : Color.clear;
                icon.SetPixel(x, 7 - y, ink);
            }
        icon.Apply();
        return icon;
    }

    private void OnDestroy()
    {
        // The icons were made at runtime, so release their textures with this scene.
        if (icons != null) foreach (Texture2D icon in icons) Destroy(icon);
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
