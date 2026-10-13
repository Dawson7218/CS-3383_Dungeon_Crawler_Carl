using UnityEngine;

public class MVPDisplay : MonoBehaviour
{
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private PlayerController playerController;
    private LootSystem lootSystem;
    private Item currentItem;
    private DungeonGenerator generator;
    private Room currentRoom;

    void Start()
    {
        generator = GetComponent<DungeonGenerator>();

        // Generate one room.
        currentRoom = generator.GenerateRun(12345, 1);

        lootSystem = GetComponent<LootSystem>();

        currentItem = lootSystem.RollDrop(currentRoom.getRoomType());

        Debug.Log("Generated item: " + currentItem.getItemKind());

        Debug.Log("MVP started!");
        Debug.Log("Room type: " + currentRoom.getRoomType());
    }

    private Vector2 playerPosition = new Vector2(230, 150);
    private float playerSpeed = 200f;
    private Vector2 enemyPosition = new Vector2(350, 150);
    private Vector2 itemPosition = new Vector2(100, 250);


    private Vector2 GetMovementInput()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            direction.y -= 1;

        if (Input.GetKey(KeyCode.S))
            direction.y += 1;

        if (Input.GetKey(KeyCode.A))
            direction.x -= 1;

        if (Input.GetKey(KeyCode.D))
            direction.x += 1;

        return direction.normalized;
    }



//    void Update()
//        {
//            Vector2 direction = GetMovementInput();

//            // Temporary movement until Rinab's Move() is ready.
//            playerPosition += direction.normalized * playerSpeed * Time.deltaTime;

//            // Keep the player inside the room.
//            playerPosition.x = Mathf.Clamp(playerPosition.x, 0, 460);
//            playerPosition.y = Mathf.Clamp(playerPosition.y, 40, 310);
//    }


    void OnGUI()
    {

        float scaleX = Screen.width / 600f;
        float scaleY = Screen.height / 450f;

        GUI.matrix = Matrix4x4.Scale(
            new Vector3(scaleX, scaleY, 1f)
        );


        // Draw the room background.
        float roomWidth = 500;
        float roomHeight = 350;

        float x = (600 - roomWidth) / 2;
        float y = (450 - roomHeight) / 2;

        GUI.Box(new Rect(x, y, roomWidth, roomHeight),
                "Dungeon Room");

            GUI.color = Color.white;

        GUI.Box(
            new Rect(
                x + roomWidth - 160,
                y + 10,
                150,
                35
            ),
            "Score: " + economyManager.GetBalance()
        );

        // Draw the player as a blue square.
        GUI.color = Color.blue;
    GUI.Box(new Rect(
        x + playerPosition.x,
        y + playerPosition.y,
        40,
        40
    ), "");
        GUI.color = Color.white;

        // Draw enemy as a red square.
//    GUI.color = Color.red;

//    GUI.Box(new Rect(
//        x + enemyPosition.x,
//        y + enemyPosition.y,
//        40,
//        40
//    ), "");

    GUI.color = Color.white;

    if (currentItem != null)
    {
        // Draw item as a yellow square.
        GUI.color = Color.yellow;

        GUI.Box(new Rect(
           x + itemPosition.x,
            y + itemPosition.y,
            20,
            20
        ), "");



        GUI.color = Color.white;
    
        GUI.Label(new Rect(
        x + itemPosition.x,
        y + itemPosition.y + 20,
        150, 25
    ), currentItem.getItemKind().ToString());
    }

    // Temporary item placeholder
//    GUI.color = Color.yellow;

//    GUI.Box(new Rect(
//        x + itemPosition.x,
//        y + itemPosition.y,
//        20,
//        20
//    ), "");

//    GUI.color = Color.white;

//    GUI.Label(new Rect(
//        x + itemPosition.x,
//        y + itemPosition.y + 20,
//        100, 25
//    ), "Item");

//    GUI.Label(new Rect(
//        x + enemyPosition.x,
//        y + enemyPosition.y + 40,
//        100, 25
//    ), "Enemy");

    // Display room information.
    if (currentRoom != null)
        {
            GUI.Label(new Rect(x + 10, y + 30, 300, 30),
                "Room Type: " + currentRoom.getRoomType());
        }

    if (Time.timeScale == 0f)
        {
            GUI.color = Color.white;

            GUI.Box(
                new Rect(
                    x + roomWidth / 2 - 100,
                    y + roomHeight / 2 - 25,
                    200,
                    50
                ),
                "GAME PAUSED"
            );
        }
    }


    private void LateUpdate()
    {
        if (playerController == null)
            return;

        // Get the position calculated by Rinab's movement system.
        Vector2 position = playerController.GetPosition();

        // Convert Unity world coordinates to our GUI room coordinates.
        playerPosition.x = 230f + position.x * 20f;
        playerPosition.y = 150f - position.y * 20f;
    }

}
