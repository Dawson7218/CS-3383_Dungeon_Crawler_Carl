using NUnit.Framework;
using UnityEngine;

public class RoomConnectionTests
{
    [Test]
    public void MissingNeighborReturnsNullAndIsNotReportedAsConnected()
    {
        var room = new Room();
        Assert.That(room.GetNeighbor(Direction.North), Is.Null);
        Assert.That(room.HasNeighbor(Direction.North), Is.False);
    }

    [Test]
    public void AddingOneNeighborDoesNotConnectOtherDirections()
    {
        var room = new Room();
        var neighbor = new CombatRoom();
        room.AddNeighbor(Direction.East, neighbor);
        Assert.That(room.GetNeighbor(Direction.East), Is.SameAs(neighbor));
        Assert.That(room.HasNeighbor(Direction.East), Is.True);
        Assert.That(room.HasNeighbor(Direction.North), Is.False);
        Assert.That(room.HasNeighbor(Direction.South), Is.False);
        Assert.That(room.HasNeighbor(Direction.West), Is.False);
    }

    [Test]
    public void SeparateRoomsDoNotShareTheirNeighborCollections()
    {
        var first = new Room();
        var second = new Room();
        first.AddNeighbor(Direction.North, new Room());
        Assert.That(first.HasNeighbor(Direction.North), Is.True);
        Assert.That(second.HasNeighbor(Direction.North), Is.False);
    }

    [Test]
    public void ReplacingANeighborPreservesTheOtherConnections()
    {
        var room = new Room();
        var north = new Room();
        var replacement = new Room();
        room.AddNeighbor(Direction.North, north);
        room.AddNeighbor(Direction.East, new Room());
        room.AddNeighbor(Direction.East, replacement);
        Assert.That(room.GetNeighbor(Direction.East), Is.SameAs(replacement));
        Assert.That(room.GetNeighbor(Direction.North), Is.SameAs(north));
    }

    [Test]
    public void ChangingRoomMetadataDoesNotEraseItsConnections()
    {
        var room = new Room();
        var neighbor = new Room();
        room.AddNeighbor(Direction.West, neighbor);
        room.SetPosition(new Vector2Int(-4, 7));
        room.SetRoomType(RoomType.TreasureRoom);
        Assert.That(room.GetPosition(), Is.EqualTo(new Vector2Int(-4, 7)));
        Assert.That(room.getRoomType(), Is.EqualTo(RoomType.TreasureRoom));
        Assert.That(room.GetNeighbor(Direction.West), Is.SameAs(neighbor));
    }
}
