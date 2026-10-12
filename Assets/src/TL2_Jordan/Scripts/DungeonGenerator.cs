using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private List<Room> rooms = new List<Room>();

    public Room GenerateRun(int seed, int roomCount)
    {
        rooms.Clear();

        System.Random random = new System.Random(seed);

        // Always generate at least one room.
        roomCount = Mathf.Max(1, roomCount);

        for (int i = 0; i < roomCount; i++)
        {
            Room room;

            // Choose from three basic room types.
            int choice = random.Next(3);

            if (choice == 0)
            {
                room = new CombatRoom();
                room.SetRoomType(RoomType.CombatRoom);
            }
            else if (choice == 1)
            {
                room = new TreaureRoom();
                room.SetRoomType(RoomType.TreasureRoom);
            }
            else
            {
                room = new CorridorRoom();
                room.SetRoomType(RoomType.CorridorRoom);
            }

            room.GenerateLayout();
            rooms.Add(room);
        }

        Debug.Log("Generated " + rooms.Count + " rooms.");

        return rooms[0];
    }

    public List<Room> GetAllRooms()
    {
        return rooms;
    }
}
