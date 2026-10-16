
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private RoomType roomType;
    private Vector2Int gridPosition;

    private Dictionary<Direction, Room> neighbors = new Dictionary<Direction, Room>();

    public void SetRoomType(RoomType type)
    {
        roomType = type;
    }

    public RoomType getRoomType()
    {
        return roomType;
    }

    public void SetPosition(Vector2Int position)
    {
        gridPosition = position;
    }

    public Vector2Int GetPosition()
    {
        return gridPosition;
    }

    // Add a connection to another room.
    public void AddNeighbor(Direction direction, Room room)
    {
        neighbors[direction] = room;
    }

    // Retrieve the room in a given direction.
    // Returns null if there is no connection.
    public Room GetNeighbor(Direction direction)
    {
        if (neighbors.TryGetValue(direction, out Room room))
        {
            return room;
        }

        return null;
    }

    // Check whether a connection exists.
    public bool HasNeighbor(Direction direction)
    {
        return neighbors.ContainsKey(direction);
    }

    public virtual void GenerateLayout()
    {
        return;
    }

    public virtual string printString()
    {
        return "Yo mama";
    }
}
