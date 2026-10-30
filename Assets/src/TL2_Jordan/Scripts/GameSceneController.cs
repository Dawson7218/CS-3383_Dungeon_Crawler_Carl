
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [Header("Dungeon System")]
    [SerializeField] private DungeonGenerator generator;
    [SerializeField] private DungeonRoomVisualizer roomVisualizer;

    [Header("Other Game Systems")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private LootSystem lootSystem;
    [SerializeField] private Camera mainCamera;

    [Header("Dungeon Settings")]
    [SerializeField, Min(2)] private int roomCount = 20;
    [SerializeField] private int seed = 12345;

    private Room currentRoom;
    private Item currentItem;

    private readonly Vector2 itemPosition =
        new Vector2(100f, 250f);

    private void Start()
    {
        // Find existing game systems if not assigned.
        if (generator == null)
            generator = GetComponent<DungeonGenerator>();

        if (generator == null)
            generator = FindAnyObjectByType<DungeonGenerator>();

        if (roomVisualizer == null)
            roomVisualizer =
                FindAnyObjectByType<DungeonRoomVisualizer>();

        if (playerController == null)
            playerController =
                FindAnyObjectByType<PlayerController>();

        if (economyManager == null)
            economyManager = EconomyManager.Instance;

        if (lootSystem == null)
            lootSystem = LootSystem.Instance;

        if (generator == null)
        {
            Debug.LogError(
                "GameSceneController needs a DungeonGenerator.",
                this
            );

            enabled = false;
            return;
        }

        // Subscribe to room transitions.
        generator.OnRoomChanged += HandleRoomChanged;

        // Reuse an existing dungeon if already generated.
        currentRoom = generator.GetCurrentRoom();

        if (currentRoom == null)
        {
            currentRoom = generator.GenerateRun(
                seed,
                Mathf.Max(2, roomCount)
            );
        }

        // Instantiate the room prefabs.
        if (roomVisualizer != null)
        {
            roomVisualizer.BuildDungeon(
                generator.GetAllRooms()
            );
            HandleRoomChanged(generator.GetCurrentRoom());
        }
        else
        {
            Debug.LogWarning(
                "DungeonRoomVisualizer not found!",
                this
            );
        }

        RefreshItem();

        if (playerController == null)
            Debug.LogWarning("PlayerController not found.");

        if (economyManager == null)
            Debug.LogWarning("EconomyManager not found.");

        if (lootSystem == null)
            Debug.LogWarning("LootSystem not found.");

        Debug.Log("GameScene started!");
        Debug.Log(
            "Generated " + generator.GetAllRooms().Count +
            " rooms."
        );
    }

    private void Update()
    {
        if (generator == null)
            return;

        // Arrow keys test logical room transitions.
        Direction direction;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            direction = Direction.North;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            direction = Direction.South;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            direction = Direction.West;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            direction = Direction.East;
        else
            return;

        Room nextRoom;

        if (generator.TryTransition(
            direction,
            out nextRoom))
        {
            GameObject roomObject = null;

            if (roomVisualizer != null)
            {
                roomObject =
                    roomVisualizer.GetRoomObject(nextRoom);
            }

            Debug.Log(
                "Entered " + nextRoom.getRoomType() +
                " at " + nextRoom.GetPosition() +
                ". GameObject: " +
                (roomObject != null
                    ? roomObject.name
                    : "None")
            );
        }
        else
        {
            Debug.Log(
                "No connected room to the " + direction
            );
        }
    }


    private void HandleRoomChanged(Room room)
    {
        currentRoom = room;
        RefreshItem();

        if (roomVisualizer == null)
            return;

        GameObject roomObject =
            roomVisualizer.GetRoomObject(room);

        if (roomObject == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector3 position = roomObject.transform.position;

            mainCamera.transform.position = new Vector3(
                position.x,
                position.y,
                mainCamera.transform.position.z
            );
        }
    }


    private void RefreshItem()
    {
        // Temporary loot placeholder from the MVP.
        currentItem =
            lootSystem != null && currentRoom != null
            ? lootSystem.RollDrop(currentRoom.getRoomType())
            : null;
    }

    private void OnDestroy()
    {
        if (generator != null)
        {
            generator.OnRoomChanged -= HandleRoomChanged;
        }
    }


    private void SpawnDungeonLoot()
    {
        if (lootSystem == null || roomVisualizer == null ||
            generator == null)
        {
            Debug.LogWarning(
                "Cannot spawn loot: a required system is missing."
            );
            return;
        }

        int treasureRooms = 0;

        foreach (Room room in generator.GetAllRooms())
        {
            // For now, spawn loot only in treasure rooms.
            if (room.getRoomType() != RoomType.TreasureRoom)
                continue;

            GameObject roomObject =
                roomVisualizer.GetRoomObject(room);

            if (roomObject == null)
                continue;

            // Spawn at the center of this room.
            Vector3 center = roomObject.transform.position;

            lootSystem.GenerateLootAt(
                room.getRoomType(),
                new Vector2(center.x, center.y)
            );

            treasureRooms++;
        }

        Debug.Log(
            "Requested loot generation in " +
            treasureRooms + " treasure rooms."
        );
    }


//    private void OnGUI()
//    {
//        Matrix4x4 previousMatrix = GUI.matrix;
//        Color previousColor = GUI.color;

//        GUI.matrix = Matrix4x4.Scale(
//            new Vector3(
//                Screen.width / 600f,
//                Screen.height / 450f,
//                1f
//            )
//        );

//        const float x = 50f;
//        const float y = 50f;

        // Temporary debug interface.
//        GUI.Box(
//            new Rect(x, y, 500f, 350f),
//            "Dungeon Room"
//        );

        // Display score.
//        string balance = economyManager != null
//            ? economyManager.GetBalance().ToString()
//            : "N/A";

//        GUI.Box(
//            new Rect(x + 340f, y + 10f, 150f, 35f),
//            "Score: " + balance
//        );

        // Display player marker.
//        Vector2 guiPlayer = new Vector2(230f, 150f);

//        if (playerController != null)
//        {
//            Vector2 world = playerController.GetPosition();

//            guiPlayer = new Vector2(
//                230f + world.x * 20f,
//                150f - world.y * 20f
//            );
//        }

//        GUI.color = Color.blue;

//        GUI.Box(
//            new Rect(
//                x + guiPlayer.x,
//                y + guiPlayer.y,
//                40f,
//                40f
//            ),
//            ""
//        );

//        GUI.color = Color.white;

        // Display temporary loot information.
//        if (currentItem != null)
//        {
//            GUI.color = Color.yellow;

//            GUI.Box(
//                new Rect(
//                    x + itemPosition.x,
//                    y + itemPosition.y,
//                    20f,
//                    20f
//                ),
//                ""
//            );

//            GUI.color = Color.white;

//            GUI.Label(
//                new Rect(
//                    x + itemPosition.x,
//                    y + itemPosition.y + 20f,
//                    150f,
//                    25f
//                ),
//                currentItem.getItemKind().ToString()
//            );
//        }

        // Display current room information.
//        if (currentRoom != null)
//        {
//            GUI.Label(
//                new Rect(x + 10f, y + 30f, 320f, 30f),
//                "Room: " +
//                currentRoom.getRoomType() +
//                " " + currentRoom.GetPosition()
//            );
//        }

        // Display pause notification.
//        if (Time.timeScale == 0f)
//        {
//            GUI.Box(
//                new Rect(200f, 200f, 200f, 50f),
//                "GAME PAUSED"
//            );
//        }

        // Restore GUI settings.
//        GUI.color = previousColor;
//        GUI.matrix = previousMatrix;
//    }
}
