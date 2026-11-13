
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomVisualizer : MonoBehaviour
{
    public event System.Action<Room, GameObject> OnRoomCreated;

    [Header("Room Prefabs")]
    [SerializeField] private GameObject combatRoomPrefab;
    [SerializeField] private GameObject treasureRoomPrefab;
    [SerializeField] private GameObject corridorRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;

    [Header("Room Spacing")]
    [SerializeField] private float roomWidth = 20f;
    [SerializeField] private float roomHeight = 12f;

    // Associate logical rooms with their Unity GameObjects.
    private Dictionary<Room, GameObject> roomObjects =
        new Dictionary<Room, GameObject>();

    public void BuildDungeon(List<Room> rooms)
    {
        // Remove rooms from a previous generation.
        ClearDungeon();

        if (rooms == null)
        {
            Debug.LogError("Room list is null!");
            return;
        }

        foreach (Room room in rooms)
        {
            // Get the correct prefab for the room type.
            GameObject prefab = GetPrefab(
                room.getRoomType()
            );

            if (prefab == null)
            {
                Debug.LogError(
                    "Missing prefab for " +
                    room.getRoomType()
                );

                continue;
            }

            // Convert grid position to world position.
            Vector2Int position = room.GetPosition();

            Vector3 worldPosition = new Vector3(
                position.x * roomWidth,
                position.y * roomHeight,
                0f
            );

            // Instantiate the room.
            GameObject roomObject = Instantiate(
                prefab,
                worldPosition,
                Quaternion.identity,
                transform
            );

            // Give the object a descriptive name.
            roomObject.name =
                room.getRoomType() + "_" +
                position.x + "_" + position.y;

            // Associate the GameObject with its logical room.
            roomObjects.Add(room, roomObject);

            RoomView roomView = roomObject.GetComponent<RoomView>();
            if (roomView != null)
                roomView.ConfigureDoorways(room);
            else
                Debug.LogWarning("Room prefab needs a RoomView to configure doorways.", roomObject);

            OnRoomCreated?.Invoke(room, roomObject);
        }

        Debug.Log(
            "Dungeon visualizer instantiated " +
            roomObjects.Count + " rooms."
        );
    }

    private GameObject GetPrefab(RoomType type)
    {
        switch (type)
        {
            case RoomType.CombatRoom:
                return combatRoomPrefab;

            case RoomType.TreasureRoom:
                return treasureRoomPrefab;

            case RoomType.CorridorRoom:
                return corridorRoomPrefab;

            case RoomType.BossRoom:
                return bossRoomPrefab;

            default:
                Debug.LogWarning(
                    "Unknown room type: " + type
                );

                return null;
        }
    }

    public GameObject GetRoomObject(Room room)
    {
        if (room != null &&
            roomObjects.TryGetValue(
                room,
                out GameObject roomObject))
        {
            return roomObject;
        }

        return null;
    }

    public Room GetRoomAtWorldPosition(Vector2 worldPosition)
    {
        float halfWidth = roomWidth * 0.5f;
        float halfHeight = roomHeight * 0.5f;

        foreach (var entry in roomObjects)
        {
            if (entry.Value == null)
                continue;

            Vector3 center = entry.Value.transform.position;

            // Include the lower edges and exclude the upper edges so a shared
            // doorway belongs to exactly one room, including at negative coordinates.
            if (worldPosition.x >= center.x - halfWidth &&
                worldPosition.x < center.x + halfWidth &&
                worldPosition.y >= center.y - halfHeight &&
                worldPosition.y < center.y + halfHeight)
            {
                return entry.Key;
            }
        }

        return null;
    }

    public void ClearDungeon()
    {
        foreach (GameObject roomObject in roomObjects.Values)
        {
            if (roomObject != null)
            {
                // Stop old encounters immediately; Destroy completes at the end of the frame.
                roomObject.SetActive(false);
                Destroy(roomObject);
            }
        }

        roomObjects.Clear();
    }
}
