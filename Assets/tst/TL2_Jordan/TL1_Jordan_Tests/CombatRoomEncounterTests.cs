using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CombatRoomEncounterTests
{
    private GameObject room;
    private GameObject player;
    private GameObject enemyPrefab;
    private CombatRoomEncounter encounter;

    [SetUp]
    public void SetUp()
    {
        room = new GameObject("TestCombatRoom");
        room.transform.position = new Vector3(2000f, -2000f, 0f);
        player = new GameObject("TestPlayer");
        player.transform.position = room.transform.position + Vector3.left * 9f;
        enemyPrefab = new GameObject("TestEnemyPrefab");
        enemyPrefab.transform.position = new Vector3(5000f, 5000f, 0f);
        enemyPrefab.AddComponent<EnemyController>();
        encounter = room.AddComponent<CombatRoomEncounter>();
        SetPrefab(encounter);
        Physics2D.SyncTransforms();
    }

    private void SetPrefab(CombatRoomEncounter target)
    {
        typeof(CombatRoomEncounter).GetField("enemyPrefab",
            BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, enemyPrefab);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(room);
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemyPrefab);
        Physics2D.SyncTransforms();
    }

    [Test]
    public void FirstEntrySpawnsThreeEnemiesInsideTheRoomAndTargetsThePlayer()
    {
        encounter.TrySpawn(player.transform);
        EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();
        Assert.That(encounter.HasSpawned, Is.True);
        Assert.That(enemies.Length, Is.EqualTo(3));

        foreach (EnemyController enemy in enemies)
        {
            Vector3 local = enemy.transform.localPosition;
            Assert.That(Mathf.Abs(local.x), Is.LessThanOrEqualTo(3f));
            Assert.That(Mathf.Abs(local.y), Is.LessThanOrEqualTo(2f));
            Assert.That(Vector2.Distance(enemy.transform.position, player.transform.position),
                Is.GreaterThanOrEqualTo(3f));
            Assert.That(typeof(EnemyController).GetField("player",
                BindingFlags.Instance | BindingFlags.NonPublic).GetValue(enemy),
                Is.SameAs(player.transform));
        }
    }

    [Test]
    public void ReturningDoesNotReplaceDefeatedEnemiesOrDuplicateSurvivors()
    {
        encounter.TrySpawn(player.transform);
        Object.DestroyImmediate(room.GetComponentInChildren<EnemyController>().gameObject);
        encounter.TrySpawn(player.transform);
        Assert.That(room.GetComponentsInChildren<EnemyController>().Length, Is.EqualTo(2));
    }

    [Test]
    public void EachRoomHasItsOwnFirstEntryState()
    {
        GameObject secondRoom = new GameObject("SecondCombatRoom");
        try
        {
            secondRoom.transform.position = room.transform.position + Vector3.right * 20f;
            var secondEncounter = secondRoom.AddComponent<CombatRoomEncounter>();
            SetPrefab(secondEncounter);
            encounter.TrySpawn(player.transform);
            Assert.That(secondEncounter.HasSpawned, Is.False);
            secondEncounter.TrySpawn(player.transform);
            Assert.That(secondRoom.GetComponentsInChildren<EnemyController>().Length, Is.EqualTo(3));
            Assert.That(room.GetComponentsInChildren<EnemyController>().Length, Is.EqualTo(3));
        }
        finally
        {
            Object.DestroyImmediate(secondRoom);
        }
    }

    [Test]
    public void MissingPlayerDoesNotConsumeTheFirstEntry()
    {
        encounter.TrySpawn(null);
        Assert.That(encounter.HasSpawned, Is.False);
        Assert.That(room.GetComponentsInChildren<EnemyController>(), Is.Empty);
        encounter.TrySpawn(player.transform);
        Assert.That(room.GetComponentsInChildren<EnemyController>().Length, Is.EqualTo(3));
    }

    [Test]
    public void BlockedSpawnPositionsAreSkipped()
    {
        var obstacle = new GameObject("BlockedArena");
        obstacle.transform.SetParent(room.transform, false);
        obstacle.AddComponent<BoxCollider2D>().size = new Vector2(8f, 6f);
        Physics2D.SyncTransforms();
        LogAssert.Expect(LogType.Warning,
            "Not enough clear positions for all requested enemies in TestCombatRoom.");
        encounter.TrySpawn(player.transform);
        Assert.That(room.GetComponentsInChildren<EnemyController>(), Is.Empty);
        Assert.That(encounter.HasSpawned, Is.True);
    }

    [Test]
    public void DestroyingTheRoomAlsoDestroysItsEnemies()
    {
        encounter.TrySpawn(player.transform);
        EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();
        Object.DestroyImmediate(room);
        foreach (EnemyController enemy in enemies)
            Assert.That(enemy == null, Is.True);
    }

    [Test]
    public void EnemySizeIsPreservedButCollisionFitsItsFeet()
    {
        var texture = new Texture2D(20, 40);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 20, 40),
            new Vector2(0.5f, 0.5f), 100f);
        try
        {
            enemyPrefab.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            enemyPrefab.AddComponent<SpriteRenderer>().sprite = sprite;
            enemyPrefab.AddComponent<BoxCollider2D>().size = Vector2.one;
            encounter.TrySpawn(player.transform);

            foreach (EnemyController enemy in room.GetComponentsInChildren<EnemyController>())
            {
                Assert.That(enemy.transform.localScale, Is.EqualTo(enemyPrefab.transform.localScale));
                BoxCollider2D collider = enemy.GetComponent<BoxCollider2D>();
                Assert.That(collider.size.x, Is.EqualTo(0.16f).Within(0.001f));
                Assert.That(collider.size.y, Is.EqualTo(0.14f).Within(0.001f));
                Assert.That(collider.offset.y, Is.EqualTo(-0.12f).Within(0.001f));
            }
        }
        finally
        {
            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(texture);
        }
    }

    [Test]
    public void EnemyBodiesCannotShoveThePlayerButOtherContactsArePreserved()
    {
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();
        CircleCollider2D playerTrigger = player.AddComponent<CircleCollider2D>();
        playerTrigger.isTrigger = true;
        enemyPrefab.AddComponent<BoxCollider2D>();
        enemyPrefab.AddComponent<CircleCollider2D>().isTrigger = true;
        var wall = new GameObject("TestWall");
        wall.transform.SetParent(room.transform, false);
        wall.transform.localPosition = new Vector3(9f, 5f, 0f);
        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        Physics2D.SyncTransforms();

        encounter.TrySpawn(player.transform);
        EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();
        Assert.That(enemies.Length, Is.EqualTo(3));
        foreach (EnemyController enemy in enemies)
        {
            BoxCollider2D enemyBody = enemy.GetComponent<BoxCollider2D>();
            CircleCollider2D enemyTrigger = enemy.GetComponent<CircleCollider2D>();
            Assert.That(Physics2D.GetIgnoreCollision(enemyBody, playerCollider), Is.True);
            Assert.That(Physics2D.GetIgnoreCollision(enemyBody, wallCollider), Is.False);
            Assert.That(Physics2D.GetIgnoreCollision(enemyBody, playerTrigger), Is.False);
            Assert.That(Physics2D.GetIgnoreCollision(enemyTrigger, playerCollider), Is.False);
        }
        Assert.That(Physics2D.GetIgnoreCollision(playerCollider, wallCollider), Is.False);
    }

    [Test]
    public void TransformDrivenPlayerDoesNotRetainCollisionMomentum()
    {
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.gravityScale = 1f;
        body.linearVelocity = new Vector2(3f, -2f);
        body.angularVelocity = 30f;
        Vector3 position = player.transform.position;
        var controller = room.AddComponent<GameSceneController>();
        typeof(GameSceneController).GetField("playerBody",
            BindingFlags.Instance | BindingFlags.NonPublic).SetValue(controller, body);

        typeof(GameSceneController).GetMethod("FixedUpdate",
            BindingFlags.Instance | BindingFlags.NonPublic).Invoke(controller, null);

        Assert.That(body.gravityScale, Is.Zero);
        Assert.That(body.linearVelocity, Is.EqualTo(Vector2.zero));
        Assert.That(body.angularVelocity, Is.Zero);
        Assert.That(player.transform.position, Is.EqualTo(position));
    }
}
