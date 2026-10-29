using UnityEngine;

public class RoomView : MonoBehaviour
{
    [SerializeField] private RoomType roomType;

    public RoomType GetRoomType()
    {
        return roomType;
    }
}