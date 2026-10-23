
using UnityEngine;

public class DungeonEventTester : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;

    private void OnEnable()
    {
        if (dungeonGenerator == null)
            return;

        dungeonGenerator.OnRoomGenerated += HandleRoomGenerated;
        dungeonGenerator.OnRoomChanged += HandleRoomChanged;
    }

    private void OnDisable()
    {
        if (dungeonGenerator == null)
            return;

        dungeonGenerator.OnRoomGenerated -= HandleRoomGenerated;
        dungeonGenerator.OnRoomChanged -= HandleRoomChanged;
    }

    private void HandleRoomGenerated(Room room)
    {
        Debug.Log(
            "EVENT: Room generated at " +
            room.GetPosition() +
            " (" + room.getRoomType() + ")"
        );
    }

    private void HandleRoomChanged(Room room)
    {
        Debug.Log(
            "EVENT: Entered room at " +
            room.GetPosition()
        );
    }
}
