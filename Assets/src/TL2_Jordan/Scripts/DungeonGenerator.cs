using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private List<Room> rooms = new List<Room>();

    [SerializeField] private GameObject combatRoomPrefab;
    [SerializeField] private float roomSpacingX = 20f;
    [SerializeField] private float roomSpacingY = 12f;

    private void SpawnRoomPrefabs()
    {
        foreach (Room room in rooms)
        {
            Vector2Int gridPos = room.GetPosition();

            UnityEngine.Vector3 worldPos = new UnityEngine.Vector3(
                gridPos.x * roomSpacingX,
                gridPos.y * roomSpacingY,
                0f
            );

            Instantiate(
                combatRoomPrefab,
                worldPos,
                UnityEngine.Quaternion.identity,
                transform
            );
        }
    }

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public Room GenerateRun(int seed, int roomCount)
    {
        rooms.Clear();

        System.Random random = new System.Random(seed);

        // Always generate at least one room.
        roomCount = Mathf.Max(1, roomCount);

        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        List<Vector2Int> frontier = new List<Vector2Int>();

        // Create the starting room.
        Vector2Int startPosition = Vector2Int.zero;

        Room startRoom = CreateRandomRoom(random);
        startRoom.SetPosition(startPosition);

        rooms.Add(startRoom);
        occupied.Add(startPosition);

        AddToFrontier(startPosition, occupied, frontier);

        while (rooms.Count < roomCount)
        {
            // Pick a random available position.
            int index = random.Next(frontier.Count);
            Vector2Int position = frontier[index];
            
            frontier.RemoveAt(index);
            
            //Create a random room at that position.
            Room room = CreateRandomRoom(random);
            room.SetPosition(position);

            rooms.Add(room);
            occupied.Add(position);

            // Find new available positions.
            AddToFrontier(position, occupied, frontier);

            SpawnRoomPrefabs();
        }

        //Generate each room's internal layout
        foreach (Room room in rooms)
        {
            room.GenerateLayout();

            Debug.Log( room.getRoomType() + " at " + room.GetPosition());
        }

        return rooms[0];
    }

    private Room CreateRandomRoom(System.Random random)
    {
        Room room;

        int choice = random.Next(3);

        if (choice == 0)
        {
            room = new CombatRoom();
            room.SetRoomType(RoomType.CombatRoom);
        }
        else if (choice == 1)
        {
            room = new TreasureRoom();
            room.SetRoomType(RoomType.TreasureRoom);
        }
        else
        {
            room = new CorridorRoom();
            room.SetRoomType(RoomType.CorridorRoom);
        }

        return room;
    }

    private void AddToFrontier(Vector2Int position, HashSet<Vector2Int> occupied, List<Vector2Int> frontier)
    {
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = position + direction;

            if (!occupied.Contains(neighbor) && !frontier.Contains(neighbor))
            {
                frontier.Add(neighbor);
            }
        }
    }

    private void ConnectRooms()
{
    // Create a lookup table from grid positions to rooms.
    Dictionary<Vector2Int, Room> roomMap =
        new Dictionary<Vector2Int, Room>();

    foreach (Room room in rooms)
    {
        roomMap.Add(room.GetPosition(), room);
    }

    // Examine each room's four neighboring positions.
    foreach (Room room in rooms)
    {
        Vector2Int position = room.GetPosition();

        ConnectNeighbor(room, position + Vector2Int.up,
                        Direction.North, roomMap);

        ConnectNeighbor(room, position + Vector2Int.down,
                        Direction.South, roomMap);

        ConnectNeighbor(room, position + Vector2Int.right,
                        Direction.East, roomMap);

        ConnectNeighbor(room, position + Vector2Int.left,
                        Direction.West, roomMap);
    }
}

    private void ConnectNeighbor(Room room, Vector2Int neighborPosition, Direction direction,
        Dictionary<Vector2Int, Room> roomMap)
    {
        if (roomMap.TryGetValue(neighborPosition, out Room neighbor))
        {
            room.AddNeighbor(direction, neighbor);
        }
    }

    public List<Room> GetAllRooms()
    {
        return rooms;
    }

    
    void Start()
    {
        GenerateRun(12345, 12);
    }
}
