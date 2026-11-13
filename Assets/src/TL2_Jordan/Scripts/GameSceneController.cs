
using System.Collections.Generic;
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

    [Header("Dungeon Settings")]
    [SerializeField, Min(2)] private int roomCount = 20;
    [SerializeField] private bool useRandomSeed = true;
    [Tooltip("Used when Use Random Seed is off; updated to the chosen seed during a random run.")]
    [SerializeField] private int seed = 12345;

    [Header("Treasure Loot")]
    [SerializeField, Range(0f, 1f)] private float secondItemChance = 0.35f;
    [Tooltip("Distance from the room center available for loot; leaves a margin inside the 20 x 12 room.")]
    [SerializeField] private Vector2 lootSpawnHalfExtents = new Vector2(8f, 4f);

    private Room currentRoom;
    private Item currentItem;
    private Rigidbody2D playerBody;

    private readonly Vector2 itemPosition =
        new Vector2(100f, 250f);

    private void Awake()
    {
        // Room encounters replace the prototype startup encounter in this scene.
        // Disable before Start runs, without changing the teammate's script or scene.
        foreach (EncounterSpawner spawner in FindObjectsByType<EncounterSpawner>(FindObjectsInactive.Include))
        {
            if (spawner.gameObject.scene == gameObject.scene)
                spawner.enabled = false;
        }

        foreach (MVPEnemyScaler scaler in FindObjectsByType<MVPEnemyScaler>(FindObjectsInactive.Include))
        {
            if (scaler.gameObject.scene == gameObject.scene)
                scaler.enabled = false;
        }
    }

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

        if (playerController != null)
            playerBody = playerController.GetComponent<Rigidbody2D>();

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
            if (useRandomSeed)
                seed = Random.Range(0, int.MaxValue);

            Debug.Log("Dungeon seed: " + seed, this);

            currentRoom = generator.GenerateRun(
                seed,
                Mathf.Max(2, roomCount)
            );
        }

        // Instantiate the room prefabs.
        if (roomVisualizer != null)
        {
            roomVisualizer.OnRoomCreated += HandleRoomCreated;
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

    private void FixedUpdate()
    {
        if (playerBody == null)
            return;

        // PlayerController moves the Transform for walking and dashing. Physics
        // should resolve contact, but must not keep drifting from old contact impulses.
        // Revisit this adapter if the player switches to velocity-driven movement.
        playerBody.gravityScale = 0f;
        playerBody.linearVelocity = Vector2.zero;
        playerBody.angularVelocity = 0f;
    }

    private void LateUpdate()
    {
        if (generator == null || roomVisualizer == null || playerController == null)
            return;

        // Run after the player's Update movement, including dash movement.
        Room playerRoom = roomVisualizer.GetRoomAtWorldPosition(
            playerController.GetPosition()
        );

        // If the player is outside the generated map, keep the last known room.
        if (playerRoom != null)
            generator.TrySetCurrentRoom(playerRoom);
    }


    private void HandleRoomChanged(Room room)
    {
        currentRoom = room;
        RefreshItem();

        // Cinemachine follows the player; logical room changes do not move the camera.
        if (room == null || room.getRoomType() != RoomType.CombatRoom ||
            roomVisualizer == null || playerController == null)
            return;

        GameObject roomObject = roomVisualizer.GetRoomObject(room);
        if (roomObject == null)
            return;

        CombatRoomEncounter encounter = roomObject.GetComponent<CombatRoomEncounter>();
        if (encounter != null)
            encounter.TrySpawn(playerController.transform);
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
        if (roomVisualizer != null)
            roomVisualizer.OnRoomCreated -= HandleRoomCreated;

        if (generator != null)
        {
            generator.OnRoomChanged -= HandleRoomChanged;
        }
    }


    private void HandleRoomCreated(Room room, GameObject roomObject)
    {
        // Only treasure rooms receive a creation-time drop for now.
        if (room.getRoomType() != RoomType.TreasureRoom)
            return;

        if (lootSystem == null)
        {
            Debug.LogWarning(
                "Cannot spawn room loot: assign the loot system.",
                this
            );
            return;
        }

        // GenerateLootAt returns void. Remember existing items so only the drops
        // synchronously created by these calls are attached to this room.
        var existingItems = new HashSet<InWorldItem>(
            FindObjectsByType<InWorldItem>(FindObjectsInactive.Include)
        );

        int itemCount = Random.value < secondItemChance ? 2 : 1;
        for (int i = 0; i < itemCount; i++)
        {
            Vector3 position = roomObject.transform.position + new Vector3(
                Random.Range(-Mathf.Abs(lootSpawnHalfExtents.x), Mathf.Abs(lootSpawnHalfExtents.x)),
                Random.Range(-Mathf.Abs(lootSpawnHalfExtents.y), Mathf.Abs(lootSpawnHalfExtents.y)),
                0f
            );
            lootSystem.GenerateLootAt(room.getRoomType(), new Vector2(position.x, position.y));
        }

        foreach (InWorldItem drop in FindObjectsByType<InWorldItem>(FindObjectsInactive.Include))
        {
            if (existingItems.Contains(drop))
                continue;

            // Keep world position and remove room loot when the dungeon is rebuilt.
            drop.transform.SetParent(roomObject.transform, true);
            SpriteRenderer renderer = drop.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sortingOrder = 6;

            Debug.Log("Spawned loot in " + roomObject.name, drop);
        }
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
