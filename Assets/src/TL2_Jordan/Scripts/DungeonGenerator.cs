
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private List<Room> rooms = new List<Room>();
    private Room currentRoom;

    public event Action<Room> OnRoomChanged;
    public event Action<Room> OnRoomGenerated;

    [Header("Boss Room Settings")]
    [Min(1)]
    [SerializeField] private int minBossDistance = 4;

    [Header("Room Generation")]
    [Range(0f, 1f)]
    [SerializeField] private float treasureChance = 0.2f;

    [Min(0)]
    [SerializeField] private int maxTreasureRooms = 2;

    [Range(0f, 1f)]
    [SerializeField] private float combatChance = 0.7f;

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
        currentRoom = null;

        System.Random random = new System.Random(seed);
        roomCount = Mathf.Max(1, roomCount);


    Vector2Int bossPosition;

    List<Vector2Int> positions = GeneratePositions(
        random,
        roomCount,
        out bossPosition
    );


        // STEP 3: Create the appropriate room objects.
        int treasureCount = 0;

        foreach (Vector2Int position in positions)
        {
            Room room;

            if (position == bossPosition)
            {
                room = new BossRoom();
                room.SetRoomType(RoomType.BossRoom);
            }
            else if (position == Vector2Int.zero)
            {
                // The starting room is always a corridor.
                room = new CorridorRoom();
                room.SetRoomType(RoomType.CorridorRoom);
            }
            else if (treasureCount < maxTreasureRooms &&
                     random.NextDouble() < treasureChance)
            {
                room = new TreasureRoom();
                room.SetRoomType(RoomType.TreasureRoom);
                treasureCount++;
            }
            else if (random.NextDouble() < combatChance)
            {
                room = new CombatRoom();
                room.SetRoomType(RoomType.CombatRoom);
            }
            else
            {
                room = new CorridorRoom();
                room.SetRoomType(RoomType.CorridorRoom);
            }

            room.SetPosition(position);
            rooms.Add(room);
        }

        // STEP 4: Connect adjacent rooms.
        ConnectRooms();

        // STEP 5: Generate layouts.
        foreach (Room room in rooms)
        {
            room.GenerateLayout();
        }

        // Notify listeners after all layouts are complete.
        foreach (Room room in rooms)
        {
            OnRoomGenerated?.Invoke(room);
        }

        // STEP 6: Set the starting room.
        currentRoom = rooms[0];
        OnRoomChanged?.Invoke(currentRoom);

        Debug.Log(
            "Generated " + rooms.Count +
            " rooms, including " + treasureCount +
            " treasure rooms."
        );

        return currentRoom;
    }


    private List<Vector2Int> GeneratePositions(
        System.Random random,
        int roomCount,
        out Vector2Int bossPosition)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        HashSet<Vector2Int> occupied =
            new HashSet<Vector2Int>();

        List<Vector2Int> frontier = new List<Vector2Int>();

        Vector2Int start = Vector2Int.zero;
        positions.Add(start);
        occupied.Add(start);

        // Special case: a one-room dungeon.
        if (roomCount == 1)
        {
            bossPosition = start;
            return positions;
        }

        // We cannot require more transitions than
        // the number of other rooms available.
        int distance = Mathf.Clamp(
            minBossDistance, 1, roomCount - 1
        );

        // Reserve a straight path to the boss.
        for (int i = 1; i < distance; i++)
        {
            Vector2Int position = new Vector2Int(i, 0);

            positions.Add(position);
            occupied.Add(position);
        }

        bossPosition = new Vector2Int(distance, 0);
        Vector2Int reservedBossPosition = bossPosition;

        // Identify other possible room positions.
        foreach (Vector2Int position in positions)
        {
            AddToFrontier(position, occupied, frontier);
        }

        // Remove positions that would create
        // additional entrances to the boss.
        frontier.RemoveAll(position =>
            IsTooCloseToBoss(position, reservedBossPosition));

        // Randomly generate remaining rooms.
        while (positions.Count < roomCount - 1)
        {
            int index = random.Next(frontier.Count);
            Vector2Int position = frontier[index];

            frontier.RemoveAt(index);

            positions.Add(position);
            occupied.Add(position);

            AddToFrontier(position, occupied, frontier);

            frontier.RemoveAll(candidate =>
                IsTooCloseToBoss(candidate, reservedBossPosition));
        }

        // Add the reserved boss room last.
        positions.Add(bossPosition);

        return positions;
    }


    private bool IsTooCloseToBoss(
        Vector2Int position,
        Vector2Int bossPosition)
    {
        // Never place another room at the boss position.
        if (position == bossPosition)
            return true;

        // Allow the one intended entrance.
        Vector2Int entrance = bossPosition + Vector2Int.left;

        if (position == entrance)
            return false;

        // Check the four adjacent grid cells.
        foreach (Vector2Int direction in directions)
        {
            if (position == bossPosition + direction)
                return true;
        }

        return false;
    }

    private void AddToFrontier(
        Vector2Int position,
        HashSet<Vector2Int> occupied,
        List<Vector2Int> frontier)
    {
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = position + direction;

            if (!occupied.Contains(neighbor) &&
                !frontier.Contains(neighbor))
            {
                frontier.Add(neighbor);
            }
        }
    }

    private Vector2Int FindBossPosition(
        List<Vector2Int> positions,
        System.Random random)
    {
        HashSet<Vector2Int> occupied =
            new HashSet<Vector2Int>(positions);

        Queue<Vector2Int> queue =
            new Queue<Vector2Int>();

        Dictionary<Vector2Int, int> distances =
            new Dictionary<Vector2Int, int>();

        Vector2Int start = Vector2Int.zero;

        queue.Enqueue(start);
        distances[start] = 0;

        int maxDistance = 0;

        List<Vector2Int> farthestPositions =
            new List<Vector2Int>();

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int distance = distances[current];

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPositions.Clear();
                farthestPositions.Add(current);
            }
            else if (distance == maxDistance)
            {
                farthestPositions.Add(current);
            }

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;

                if (occupied.Contains(neighbor) &&
                    !distances.ContainsKey(neighbor))
                {
                    distances[neighbor] = distance + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // A one-room dungeon uses its only room as the boss.
        if (farthestPositions.Count == 0)
            return start;

        // Break ties randomly but reproducibly.
        int index = random.Next(farthestPositions.Count);
        return farthestPositions[index];
    }

    private void ConnectRooms()
    {
        Dictionary<Vector2Int, Room> roomMap =
            new Dictionary<Vector2Int, Room>();

        foreach (Room room in rooms)
        {
            roomMap.Add(room.GetPosition(), room);
        }

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

    private void ConnectNeighbor(
        Room room,
        Vector2Int position,
        Direction direction,
        Dictionary<Vector2Int, Room> roomMap)
    {
        if (roomMap.TryGetValue(position, out Room neighbor))
        {
            room.AddNeighbor(direction, neighbor);
        }
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public bool TryTransition(Direction dir, out Room nextRoom)
    {
        nextRoom = null;

        if (currentRoom == null)
            return false;

        Room neighbor = currentRoom.GetNeighbor(dir);

        if (neighbor == null)
            return false;

        currentRoom = neighbor;
        nextRoom = currentRoom;

        OnRoomChanged?.Invoke(currentRoom);

        return true;
    }

    public List<Room> GetAllRooms()
    {
        return rooms;
    }


    private void TestBossRoom()
    {
        GenerateRun(12345, 20);

        Room boss = null;

        foreach (Room room in rooms)
        {
            if (room.getRoomType() == RoomType.BossRoom)
            {
                boss = room;
                break;
            }
        }

        if (boss == null)
        {
            Debug.LogError("No boss room generated!");
            return;
        }

        int connections = 0;

        foreach (Direction direction in
                System.Enum.GetValues(typeof(Direction)))
        {
            if (boss.HasNeighbor(direction))
                connections++;
        }

        Debug.Log("Boss position: " + boss.GetPosition());
        Debug.Log("Boss connections: " + connections);

        if (connections == 1)
            Debug.Log("PASS: Boss has exactly one entrance.");
        else
            Debug.LogError("FAIL: Incorrect boss connections.");

        // The reserved path is straight, so its length
        // equals the boss's X coordinate.
        int distance = boss.GetPosition().x;

        if (distance >= minBossDistance)
            Debug.Log("PASS: Boss distance requirement met.");
        else
            Debug.LogError("FAIL: Boss is too close to start.");
    }

}
