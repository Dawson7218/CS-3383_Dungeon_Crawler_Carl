using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class DungeonGenerationTests
{
    private GameObject root;
    private DungeonGenerator generator;
    private UnityEngine.Random.State randomState;
    private static readonly Direction[] Directions =
        (Direction[])Enum.GetValues(typeof(Direction));

    [SetUp]
    public void SetUp()
    {
        randomState = UnityEngine.Random.state;
        root = new GameObject("DungeonGenerationTests");
        generator = root.AddComponent<DungeonGenerator>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
        UnityEngine.Random.state = randomState;
    }

    // Change the same serialized settings exposed by Unity's Inspector.
    private void SetSetting<T>(string name, T value)
    {
        FieldInfo field = typeof(DungeonGenerator).GetField(name,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(field, Is.Not.Null, "Missing Inspector setting: " + name);
        field.SetValue(generator, value);
    }

    private List<Room> Generate(int count = 20, int seed = 12345)
    {
        generator.GenerateRun(seed, count);
        return generator.GetAllRooms();
    }

    private static Room Boss(IEnumerable<Room> rooms)
    {
        return rooms.Single(room => room.getRoomType() == RoomType.BossRoom);
    }

    // Independently traverse the public graph; do not infer distance from X alone.
    private static Dictionary<Room, int> DistancesFrom(Room start)
    {
        var distances = new Dictionary<Room, int> { [start] = 0 };
        var frontier = new Queue<Room>();
        frontier.Enqueue(start);
        while (frontier.Count > 0)
        {
            Room room = frontier.Dequeue();
            foreach (Direction direction in Directions)
            {
                Room next = room.GetNeighbor(direction);
                if (next == null || distances.ContainsKey(next))
                    continue;
                distances[next] = distances[room] + 1;
                frontier.Enqueue(next);
            }
        }
        return distances;
    }

    private static Vector2Int Offset(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return Vector2Int.up;
            case Direction.South: return Vector2Int.down;
            case Direction.East: return Vector2Int.right;
            default: return Vector2Int.left;
        }
    }

    private static Direction Opposite(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            default: return Direction.East;
        }
    }

    private static string LayoutSignature(IEnumerable<Room> rooms)
    {
        return string.Join("|", rooms.OrderBy(room => room.GetPosition().x)
            .ThenBy(room => room.GetPosition().y)
            .Select(room => room.GetPosition() + ":" + room.getRoomType()));
    }

    [Test]
    public void BeforeGenerationThereIsNoCurrentRoom()
    {
        Assert.That(generator.GetCurrentRoom(), Is.Null);
        Assert.That(generator.GetAllRooms(), Is.Empty);
    }

    [Test]
    public void TransitionBeforeGenerationFailsWithoutADestination()
    {
        Assert.That(generator.TryTransition(Direction.East, out Room next), Is.False);
        Assert.That(next, Is.Null);
        Assert.That(generator.GetCurrentRoom(), Is.Null);
    }

    [Test]
    public void RequestedRoomCountIsPreservedForSmallAndLargeRuns()
    {
        foreach (int count in new[] { 1, 2, 5, 12, 20, 80 })
            Assert.That(Generate(count).Count, Is.EqualTo(count), "Requested " + count);
    }

    [Test]
    public void ZeroRequestedRoomsProducesOneValidRoom()
    {
        List<Room> rooms = Generate(0);
        Assert.That(rooms.Count, Is.EqualTo(1));
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(rooms[0]));
    }

    [Test]
    public void NegativeRequestedRoomsProducesOneValidRoom()
    {
        Assert.That(Generate(-15).Count, Is.EqualTo(1));
        Assert.That(generator.GetCurrentRoom(), Is.Not.Null);
    }

    [Test]
    public void StartingRoomIsAtTheGridOrigin()
    {
        Generate();
        Assert.That(generator.GetCurrentRoom().GetPosition(), Is.EqualTo(Vector2Int.zero));
    }

    [Test]
    public void MultiRoomRunStartsInACorridor()
    {
        Generate();
        Assert.That(generator.GetCurrentRoom(), Is.TypeOf<CorridorRoom>());
        Assert.That(generator.GetCurrentRoom().getRoomType(), Is.EqualTo(RoomType.CorridorRoom));
    }

    [Test]
    public void GeneratedRoomsNeverShareAGridPosition()
    {
        List<Room> rooms = Generate(80);
        Assert.That(rooms.Select(room => room.GetPosition()).Distinct().Count(), Is.EqualTo(rooms.Count));
    }

    [Test]
    public void EveryRoomIsReachableFromTheStart()
    {
        List<Room> rooms = Generate(80);
        var distances = DistancesFrom(generator.GetCurrentRoom());
        CollectionAssert.AreEquivalent(rooms, distances.Keys);
    }

    [Test]
    public void DungeonContainsExactlyOneBossRoom()
    {
        Assert.That(Generate(50).Count(room => room.getRoomType() == RoomType.BossRoom), Is.EqualTo(1));
    }

    [Test]
    public void BossRoomHasExactlyOneEntrance()
    {
        Room boss = Boss(Generate(80));
        Assert.That(Directions.Count(boss.HasNeighbor), Is.EqualTo(1));
    }

    [Test]
    public void BossMeetsTheDefaultShortestPathDistance()
    {
        Room boss = Boss(Generate());
        Assert.That(DistancesFrom(generator.GetCurrentRoom())[boss], Is.GreaterThanOrEqualTo(4));
    }

    [Test]
    public void ConfiguredBossDistanceIsHonored()
    {
        SetSetting("minBossDistance", 8);
        Room boss = Boss(Generate(40));
        Assert.That(DistancesFrom(generator.GetCurrentRoom())[boss], Is.GreaterThanOrEqualTo(8));
        Assert.That(Directions.Count(boss.HasNeighbor), Is.EqualTo(1));
    }

    [Test]
    public void SmallDungeonClampsImpossibleBossDistance()
    {
        SetSetting("minBossDistance", 100);
        Room boss = Boss(Generate(3));
        Assert.That(DistancesFrom(generator.GetCurrentRoom())[boss], Is.EqualTo(2));
        Assert.That(Directions.Count(boss.HasNeighbor), Is.EqualTo(1));
    }

    [Test]
    public void SingleRoomDungeonIsABossWithoutEntrances()
    {
        Room boss = Boss(Generate(1));
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(boss));
        Assert.That(Directions.Any(boss.HasNeighbor), Is.False);
    }

    [Test]
    public void EveryConnectionHasAReciprocalConnection()
    {
        foreach (Room room in Generate(50))
        foreach (Direction direction in Directions)
        {
            Room neighbor = room.GetNeighbor(direction);
            if (neighbor != null)
                Assert.That(neighbor.GetNeighbor(Opposite(direction)), Is.SameAs(room));
        }
    }

    [Test]
    public void ConnectionsLeadToTheCorrectAdjacentGridCell()
    {
        foreach (Room room in Generate(50))
        foreach (Direction direction in Directions)
        {
            Room neighbor = room.GetNeighbor(direction);
            if (neighbor != null)
                Assert.That(neighbor.GetPosition(), Is.EqualTo(room.GetPosition() + Offset(direction)));
        }
    }

    [Test]
    public void AllOccupiedAdjacentCellsAreConnectedAndEmptyCellsAreNot()
    {
        var byPosition = Generate(50).ToDictionary(room => room.GetPosition());
        foreach (Room room in byPosition.Values)
        foreach (Direction direction in Directions)
        {
            byPosition.TryGetValue(room.GetPosition() + Offset(direction), out Room expected);
            Assert.That(room.GetNeighbor(direction), Is.SameAs(expected));
        }
    }

    [Test]
    public void SameSeedReproducesPositionsAndRoomTypes()
    {
        string first = LayoutSignature(Generate(40, 777));
        Assert.That(LayoutSignature(Generate(40, 777)), Is.EqualTo(first));
    }

    [Test]
    public void UnityRandomActivityDoesNotChangeASeededMap()
    {
        string first = LayoutSignature(Generate(40, 777));
        UnityEngine.Random.InitState(9876);
        for (int i = 0; i < 100; i++)
            UnityEngine.Random.Range(-1000, 1000);
        Assert.That(LayoutSignature(Generate(40, 777)), Is.EqualTo(first));
    }

    [Test]
    public void DifferentSeedsCanProduceDifferentLayouts()
    {
        string first = LayoutSignature(Generate(40, 12345));
        Assert.That(LayoutSignature(Generate(40, 54321)), Is.Not.EqualTo(first));
    }

    [Test]
    public void ZeroTreasureChanceProducesNoTreasureRooms()
    {
        SetSetting("treasureChance", 0f);
        Assert.That(Generate(50).Any(room => room is TreasureRoom), Is.False);
    }

    [Test]
    public void ZeroTreasureLimitOverridesGuaranteedTreasureChance()
    {
        SetSetting("treasureChance", 1f);
        SetSetting("maxTreasureRooms", 0);
        Assert.That(Generate(50).Any(room => room is TreasureRoom), Is.False);
    }

    [Test]
    public void TreasureRoomsNeverExceedTheirConfiguredLimit()
    {
        SetSetting("treasureChance", 1f);
        SetSetting("maxTreasureRooms", 3);
        Assert.That(Generate(50).Count(room => room is TreasureRoom), Is.EqualTo(3));
    }

    [Test]
    public void GuaranteedTreasureCannotReplaceStartOrBoss()
    {
        SetSetting("treasureChance", 1f);
        SetSetting("maxTreasureRooms", 100);
        List<Room> rooms = Generate(12);
        Assert.That(rooms.Count(room => room is TreasureRoom), Is.EqualTo(10));
        Assert.That(generator.GetCurrentRoom(), Is.TypeOf<CorridorRoom>());
        Assert.That(rooms.Count(room => room is BossRoom), Is.EqualTo(1));
    }

    [Test]
    public void GuaranteedCombatFillsEligibleNonSpecialRooms()
    {
        SetSetting("treasureChance", 0f);
        SetSetting("combatChance", 1f);
        Assert.That(Generate(12).Count(room => room is CombatRoom), Is.EqualTo(10));
    }

    [Test]
    public void ZeroCombatAndTreasureChancesLeaveCorridorsAndBoss()
    {
        SetSetting("treasureChance", 0f);
        SetSetting("combatChance", 0f);
        List<Room> rooms = Generate(12);
        Assert.That(rooms.Count(room => room is CorridorRoom), Is.EqualTo(11));
        Assert.That(rooms.Count(room => room is BossRoom), Is.EqualTo(1));
    }

    [Test]
    public void LogicalRoomTypeMatchesItsConcreteSubclass()
    {
        foreach (Room room in Generate(50))
        {
            switch (room.getRoomType())
            {
                case RoomType.CombatRoom: Assert.That(room, Is.TypeOf<CombatRoom>()); break;
                case RoomType.TreasureRoom: Assert.That(room, Is.TypeOf<TreasureRoom>()); break;
                case RoomType.CorridorRoom: Assert.That(room, Is.TypeOf<CorridorRoom>()); break;
                case RoomType.BossRoom: Assert.That(room, Is.TypeOf<BossRoom>()); break;
                default: Assert.Fail("Generated an unspecified room type."); break;
            }
        }
    }

    [Test]
    public void RegenerationReplacesOldRoomsInsteadOfAccumulatingThem()
    {
        Room[] oldRooms = Generate(30).ToArray();
        List<Room> newRooms = Generate(7, 54321);
        Assert.That(newRooms.Count, Is.EqualTo(7));
        Assert.That(newRooms.Intersect(oldRooms), Is.Empty);
        Assert.That(newRooms, Does.Contain(generator.GetCurrentRoom()));
    }

    [Test]
    public void RoomGeneratedEventReportsEveryRoomExactlyOnce()
    {
        var announced = new List<Room>();
        generator.OnRoomGenerated += announced.Add;
        List<Room> rooms = Generate(20);
        Assert.That(announced.Count, Is.EqualTo(20));
        Assert.That(announced.Distinct().Count(), Is.EqualTo(20));
        CollectionAssert.AreEquivalent(rooms, announced);
    }

    [Test]
    public void GenerationSubscribersSeeTheFullyConnectedMap()
    {
        int notifications = 0;
        generator.OnRoomGenerated += room =>
        {
            notifications++;
            Assert.That(generator.GetAllRooms().Count, Is.EqualTo(20));
            Assert.That(DistancesFrom(room).Count, Is.EqualTo(20));
        };
        Generate();
        Assert.That(notifications, Is.EqualTo(20));
    }

    [Test]
    public void StartingRoomEventFollowsAllGenerationEvents()
    {
        int generatedCount = 0;
        int changedCount = 0;
        Room announcedStart = null;
        generator.OnRoomGenerated += room => generatedCount++;
        generator.OnRoomChanged += room =>
        {
            changedCount++;
            announcedStart = room;
            Assert.That(generatedCount, Is.EqualTo(20));
            Assert.That(generator.GetCurrentRoom(), Is.SameAs(room));
        };
        Room returnedStart = generator.GenerateRun(12345, 20);
        Assert.That(changedCount, Is.EqualTo(1));
        Assert.That(announcedStart, Is.SameAs(returnedStart));
    }

    [Test]
    public void BlockedTransitionDoesNotChangeRoomOrRaiseAnEvent()
    {
        Room boss = Boss(Generate());
        generator.TrySetCurrentRoom(boss);
        int changes = 0;
        generator.OnRoomChanged += room => changes++;
        Direction blocked = Directions.First(direction => !boss.HasNeighbor(direction));
        Assert.That(generator.TryTransition(blocked, out Room next), Is.False);
        Assert.That(next, Is.Null);
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(boss));
        Assert.That(changes, Is.Zero);
    }

    [Test]
    public void ValidTransitionNotifiesOnceWithTheDestination()
    {
        Generate();
        Room expected = generator.GetCurrentRoom().GetNeighbor(Direction.East);
        int changes = 0;
        Room announced = null;
        generator.OnRoomChanged += room => { changes++; announced = room; };
        Assert.That(generator.TryTransition(Direction.East, out Room next), Is.True);
        Assert.That(next, Is.SameAs(expected));
        Assert.That(generator.GetCurrentRoom(), Is.SameAs(expected));
        Assert.That(announced, Is.SameAs(expected));
        Assert.That(changes, Is.EqualTo(1));
    }

    [Test]
    public void TransitionThroughTheOppositeDoorReturnsToStart()
    {
        Generate();
        Room start = generator.GetCurrentRoom();
        Assert.That(generator.TryTransition(Direction.East, out _), Is.True);
        Assert.That(generator.TryTransition(Direction.West, out Room returned), Is.True);
        Assert.That(returned, Is.SameAs(start));
    }
}
