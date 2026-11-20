using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class RoomDoorwayTests
{
    private GameObject root;
    private RoomView view;
    private static readonly Direction[] Directions =
        (Direction[])Enum.GetValues(typeof(Direction));

    [SetUp]
    public void SetUp()
    {
        root = new GameObject("RoomDoorwayTests");
        view = root.AddComponent<RoomView>();
        foreach (Direction direction in Directions)
        {
            var wall = new GameObject(direction + "DoorBlocker");
            wall.transform.SetParent(root.transform, false);
            wall.AddComponent<BoxCollider2D>();
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
    }

    private GameObject Blocker(Direction direction)
    {
        return root.transform.Find(direction + "DoorBlocker").gameObject;
    }

    [Test]
    public void RoomWithoutNeighborsClosesAllFourDoorways()
    {
        view.ConfigureDoorways(new Room());
        foreach (Direction direction in Directions)
        {
            Assert.That(Blocker(direction).activeInHierarchy, Is.True);
            Assert.That(Blocker(direction).GetComponent<BoxCollider2D>().enabled, Is.True);
        }
    }

    [Test]
    public void EachConnectionOpensOnlyItsMatchingDoorway()
    {
        foreach (Direction connected in Directions)
        {
            var room = new Room();
            room.AddNeighbor(connected, new Room());
            view.ConfigureDoorways(room);
            foreach (Direction direction in Directions)
                Assert.That(Blocker(direction).activeInHierarchy, Is.EqualTo(direction != connected));
        }
    }

    [Test]
    public void FourWayJunctionDisablesAllDoorwayWallsAndTheirCollisions()
    {
        var room = new Room();
        foreach (Direction direction in Directions)
            room.AddNeighbor(direction, new Room());
        view.ConfigureDoorways(room);
        foreach (Direction direction in Directions)
            Assert.That(Blocker(direction).GetComponent<BoxCollider2D>().gameObject.activeInHierarchy,
                Is.False);
    }

    [Test]
    public void ReconfigurationClosesPreviouslyOpenDoors()
    {
        var firstRoom = new Room();
        firstRoom.AddNeighbor(Direction.East, new Room());
        view.ConfigureDoorways(firstRoom);
        Assert.That(Blocker(Direction.East).activeSelf, Is.False);

        var secondRoom = new Room();
        secondRoom.AddNeighbor(Direction.North, new Room());
        view.ConfigureDoorways(secondRoom);
        Assert.That(Blocker(Direction.East).activeSelf, Is.True);
        Assert.That(Blocker(Direction.North).activeSelf, Is.False);
    }

    [Test]
    public void NullRoomDoesNotChangeExistingDoorwayStates()
    {
        var room = new Room();
        room.AddNeighbor(Direction.South, new Room());
        view.ConfigureDoorways(room);
        view.ConfigureDoorways(null);
        Assert.That(Blocker(Direction.South).activeSelf, Is.False);
        Assert.That(Blocker(Direction.North).activeSelf, Is.True);
        Assert.That(Blocker(Direction.East).activeSelf, Is.True);
        Assert.That(Blocker(Direction.West).activeSelf, Is.True);
    }

    [Test]
    public void ProductionRoomVariantsHaveTheCorrectLogicalTypes()
    {
        var specifications = new[] {
            ("Combat", RoomType.CombatRoom), ("Treasure", RoomType.TreasureRoom),
            ("Corridor", RoomType.CorridorRoom), ("Boss", RoomType.BossRoom)
        };
        foreach (var specification in specifications)
        {
            string path = "Assets/prefabs/TL2_Jordan/" + specification.Item1 + "RoomPrefab.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(prefab, Is.Not.Null, path);
            RoomView roomView = prefab.GetComponent<RoomView>();
            Assert.That(roomView, Is.Not.Null, path);
            Assert.That(roomView.GetRoomType(), Is.EqualTo(specification.Item2), path);
        }
    }

    [Test]
    public void BaseRoomContainsFourSolidDoorwayBlockers()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/prefabs/TL2_Jordan/BaseRoom.prefab");
        Assert.That(prefab, Is.Not.Null);
        foreach (Direction direction in Directions)
        {
            Transform blocker = prefab.transform.Find(direction + "DoorBlocker");
            Assert.That(blocker, Is.Not.Null, direction.ToString());
            BoxCollider2D collider = blocker.GetComponent<BoxCollider2D>();
            Assert.That(collider, Is.Not.Null, direction.ToString());
            Assert.That(collider.isTrigger, Is.False);
            Assert.That(blocker.GetComponent<SpriteRenderer>(), Is.Not.Null);
        }
    }
}
