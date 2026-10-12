using UnityEngine;

public class MVPDisplay : MonoBehaviour
{
    private DungeonGenerator generator;
    private Room currentRoom;

    void Start()
    {
        generator = GetComponent<DungeonGenerator>();

        // Generate one room.
        currentRoom = generator.GenerateRun(12345, 1);

        Debug.Log("MVP started!");
        Debug.Log("Room type: " + currentRoom.getRoomType());
    }

    private Vector2 playerPosition = new Vector2(230, 150);
    private float playerSpeed = 200f;


void Update()
    {
        Vector2 direction = Vector2.zero;

        // Temporary input until Getty's GetInput() is ready.
        if (Input.GetKey(KeyCode.W))
            direction.y -= 1;

        if (Input.GetKey(KeyCode.S))
            direction.y += 1;

        if (Input.GetKey(KeyCode.A))
            direction.x -= 1;

        if (Input.GetKey(KeyCode.D))
            direction.x += 1;

        // Temporary movement until Rinab's Move() is ready.
        playerPosition += direction.normalized * playerSpeed * Time.deltaTime;

        // Keep the player inside the room.
        playerPosition.x = Mathf.Clamp(playerPosition.x, 0, 460);
        playerPosition.y = Mathf.Clamp(playerPosition.y, 40, 310);
}


    void OnGUI()
    {
        // Draw the room background.
        float roomWidth = 500;
        float roomHeight = 350;

        float x = (Screen.width - roomWidth) / 2;
        float y = (Screen.height - roomHeight) / 2;

        GUI.Box(new Rect(x, y, roomWidth, roomHeight),
                "Dungeon Room");

        // Draw the player as a blue square.
        GUI.color = Color.blue;
    GUI.Box(new Rect(
        x + playerPosition.x,
        y + playerPosition.y,
        40,
        40
    ), "");
        GUI.color = Color.white;

        // Display room information.
        if (currentRoom != null)
        {
            GUI.Label(new Rect(x + 10, y + 30, 300, 30),
                "Room Type: " + currentRoom.getRoomType());
        }
    }
}
