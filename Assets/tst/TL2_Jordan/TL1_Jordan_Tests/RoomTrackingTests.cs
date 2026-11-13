using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class RoomTrackingTests
{
    private GameObject root;
    private GameObject roomPrefab;
    private DungeonGenerator generator;
    private DungeonRoomVisualizer visualizer;
    private List<Room> visualRooms;

    [SetUp]
    public void SetUp()
    {
        root = new GameObject("RoomTrackingTests");
        generator = root.AddComponent<DungeonGenerator>();
        generator.GenerateRun(12345, 10);
        visualizer = root.AddComponent<DungeonRoomVisualizer>();

        roomPrefab = new GameObject("TestRoomPrefab");
        roomPrefab.AddComponent<RoomView>();
        foreach (string direction in new[] { "North", "South", "East", "West" })
        {
            var blocker = new GameObject(direction + "DoorBlocker");
            blocker.transform.SetParent(roomPrefab.transform);
        }

        // Supply lightweight prefabs without changing serialized production assets.
        foreach (string field in new[] {
            "combatRoomPrefab", "treasureRoomPrefab", "corridorRoomPrefab", "bossRoomPrefab"
        })
        {
            typeof(DungeonRoomVisualizer).GetField(field,
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(visualizer, roomPrefab);
        }

        visualRooms = new List<Room>();
        foreach (Vector2Int position in new[] {
            Vector2Int.zero, Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
        })
        {
            var room = new CorridorRoom();
            room.SetRoomType(RoomType.CorridorRoom);
            room.SetPosition(position);
            visualRooms.Add(room);
        }

        visualizer.BuildDungeon(visualRooms);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
        Object.DestroyImmediate(roomPrefab);
    }

    [TestCase(9.99f, 0f, 0, 0)]
    [TestCase(10f, 0f, 1, 0)]
    [TestCase(-10f, 0f, 0, 0)]
    [TestCase(-10.01f, 0f, -1, 0)]
    [TestCase(0f, 6f, 0, 1)]
    [TestCase(0f, 5.99f, 0, 0)]
    [TestCase(0f, -6f, 0, 0)]
    [TestCase(0f, -6.01f, 0, -1)]
    public void SharedEdgesBelongToExactlyOneRoom(float x, float y, int gridX, int gridY)
    {
        Room found = visualizer.GetRoomAtWorldPosition(new Vector2(x, y));
        Assert.That(found, Is.Not.Null);
        Assert.That(found.GetPosition(), Is.EqualTo(new Vector2Int(gridX, gridY)));
    }

    [TestCase(20f, 12f)] // Empty diagonal grid cell.
    [TestCase(1000f, -1000f)]
    public void EmptySpaceDoesNotSelectANearbyRoom(float x, float y)
    {
        Assert.That(visualizer.GetRoomAtWorldPosition(new Vector2(x, y)), Is.Null);
    }

    [Test]
    public void LookupUsesTheInstantiatedRoomPosition()
    {
        Room room = visualRooms[1];
        visualizer.GetRoomObject(room).transform.position = new Vector3(200f, -120f, 0f);
        Assert.That(visualizer.GetRoomAtWorldPosition(new Vector2(200f, -120f)), Is.SameAs(room));
        Assert.That(visualizer.GetRoomAtWorldPosition(new Vector2(20f, 0f)), Is.Null);
    }

    [Test]
    public void SelectingTheSameRoomDoesNotRepeatTheEvent()
    {
        Room start = generator.GetCurrentRoom();
        Room next = start.GetNeighbor(Direction.East);
        int changes = 0;
        generator.OnRoomChanged += room => changes++;

        Assert.That(generator.TrySetCurrentRoom(start), Is.True);
        Assert.That(changes, Is.Zero);
        Assert.That(generator.TrySetCurrentRoom(next), Is.True);
        Assert.That(generator.TrySetCurrentRoom(next), Is.True);
        Assert.That(changes, Is.EqualTo(1));
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(next));

        generator.TrySetCurrentRoom(start);
        Assert.That(changes, Is.EqualTo(2));
    }

    [Test]
    public void PositionTrackingCanCatchUpAfterTeleportingAcrossSeveralRooms()
    {
        Room destination = generator.GetAllRooms()[generator.GetAllRooms().Count - 1];
        Room announced = null;
        generator.OnRoomChanged += room => announced = room;

        Assert.That(generator.TrySetCurrentRoom(destination), Is.True);
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(destination));
        Assert.That(announced, Is.SameAs(destination));
    }

    [Test]
    public void InvalidRoomsLeaveTheCurrentRoomAndEventsUnchanged()
    {
        Room start = generator.GetCurrentRoom();
        int changes = 0;
        generator.OnRoomChanged += room => changes++;
        Assert.That(generator.TrySetCurrentRoom(null), Is.False);
        Assert.That(generator.TrySetCurrentRoom(new CorridorRoom()), Is.False);
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(start));
        Assert.That(changes, Is.Zero);
    }

    [Test]
    public void RoomsFromAnOldRunCannotBecomeCurrent()
    {
        Room previousRun = generator.GetCurrentRoom();
        Room newStart = generator.GenerateRun(54321, 10);
        Assert.That(generator.TrySetCurrentRoom(previousRun), Is.False);
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(newStart));
    }
}
