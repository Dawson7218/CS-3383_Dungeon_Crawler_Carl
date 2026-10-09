using UnityEngine;

public class Room
{  
    private RoomType roomType;
    public RoomType getRoomType()
    {
        return this.roomType;
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