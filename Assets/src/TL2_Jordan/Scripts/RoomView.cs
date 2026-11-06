using UnityEngine;

public class RoomView : MonoBehaviour
{
    [SerializeField] private RoomType roomType;

    public RoomType GetRoomType()
    {
        return roomType;
    }

    public void ConfigureDoorways(Room room)
    {
        if (room == null)
            return;

        // These wall pieces are inherited from BaseRoom by every room variant.
        SetDoorwayBlocked("NorthDoorBlocker", !room.HasNeighbor(Direction.North));
        SetDoorwayBlocked("SouthDoorBlocker", !room.HasNeighbor(Direction.South));
        SetDoorwayBlocked("EastDoorBlocker", !room.HasNeighbor(Direction.East));
        SetDoorwayBlocked("WestDoorBlocker", !room.HasNeighbor(Direction.West));
    }

    private void SetDoorwayBlocked(string blockerName, bool blocked)
    {
        Transform blocker = transform.Find(blockerName);
        if (blocker == null)
        {
            Debug.LogWarning("Missing doorway wall: " + blockerName, this);
            return;
        }

        // This toggles both the visible wall and its collider together.
        blocker.gameObject.SetActive(blocked);
    }
}
