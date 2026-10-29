using UnityEngine;

// Temporary integration display. Does not replace teammates' gameplay systems.
public class GameSceneController : MonoBehaviour
{
    [SerializeField] private DungeonGenerator generator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private LootSystem lootSystem;
    [SerializeField, Min(2)] private int roomCount = 12;
    [SerializeField] private int seed = 12345;

    private Room currentRoom;
    private Item currentItem;
    private readonly Vector2 itemPosition = new Vector2(100f, 250f);

    [System.Obsolete]
    private void Start()
    {
        if (generator == null) generator = GetComponent<DungeonGenerator>();
        if (generator == null) generator = FindFirstObjectByType<DungeonGenerator>();
        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();
        if (economyManager == null) economyManager = EconomyManager.Instance;
        if (lootSystem == null) lootSystem = LootSystem.Instance;

        if (generator == null)
        {
            Debug.LogError("MVPDisplay needs a DungeonGenerator.", this);
            enabled = false;
            return;
        }

        // Generate only if a different script has not already generated a run.
        currentRoom = generator.GetCurrentRoom();
        if (currentRoom == null)
            currentRoom = generator.GenerateRun(seed, Mathf.Max(2, roomCount));

        generator.OnRoomChanged += HandleRoomChanged;
        RefreshItem();

        if (playerController == null)
            Debug.LogWarning("No PlayerController found; using a static GUI player marker.", this);
        if (economyManager == null)
            Debug.LogWarning("No EconomyManager found; score unavailable.", this);
        if (lootSystem == null)
            Debug.LogWarning("No LootSystem found; loot unavailable.", this);
    }

    private void OnDestroy()
    {
        if (generator != null)
            generator.OnRoomChanged -= HandleRoomChanged;
    }

    private void HandleRoomChanged(Room room)
    {
        currentRoom = room;
        RefreshItem();
    }

    private void RefreshItem()
    {
        // The teammate's RollDrop method is explicitly marked as an MVP placeholder.
        // Keep it isolated here until loot tables are configured for real gameplay.
        currentItem = lootSystem != null && currentRoom != null
            ? lootSystem.RollDrop(currentRoom.getRoomType())
            : null;
    }

    private void OnGUI()
    {
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        GUI.matrix = Matrix4x4.Scale(new Vector3(Screen.width / 600f, Screen.height / 450f, 1f));

        const float x = 50f;
        const float y = 50f;
        GUI.Box(new Rect(x, y, 500f, 350f), "Dungeon Room");

        string balance = economyManager != null ? economyManager.GetBalance().ToString() : "N/A";
        GUI.Box(new Rect(x + 340f, y + 10f, 150f, 35f), "Score: " + balance);

        Vector2 guiPlayer = new Vector2(230f, 150f);
        if (playerController != null)
        {
            Vector2 world = playerController.GetPosition();
            guiPlayer = new Vector2(230f + world.x * 20f, 150f - world.y * 20f);
        }

        GUI.color = Color.blue;
        GUI.Box(new Rect(x + guiPlayer.x, y + guiPlayer.y, 40f, 40f), "");
        GUI.color = Color.white;

        if (currentItem != null)
        {
            GUI.color = Color.yellow;
            GUI.Box(new Rect(x + itemPosition.x, y + itemPosition.y, 20f, 20f), "");
            GUI.color = Color.white;
            GUI.Label(new Rect(x + itemPosition.x, y + itemPosition.y + 20f, 150f, 25f),
                currentItem.getItemKind().ToString());
        }

        if (currentRoom != null)
            GUI.Label(new Rect(x + 10f, y + 30f, 320f, 30f),
                "Room: " + currentRoom.getRoomType() + " " + currentRoom.GetPosition());

        if (Time.timeScale == 0f)
            GUI.Box(new Rect(200f, 200f, 200f, 50f), "GAME PAUSED");

        GUI.color = previousColor;
        GUI.matrix = previousMatrix;
    }
}
