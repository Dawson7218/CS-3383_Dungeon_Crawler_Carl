using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossProjectileTests
{
    private GameObject projectileObject;
    private BossProjectile projectile;
    private Rigidbody2D body;
    private float previousTimeScale;

    [SetUp]
    public void SetUp()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 1f;

        projectileObject = new GameObject("TestBossProjectile");

        // Required physics components are added automatically.
        projectile = projectileObject.AddComponent<BossProjectile>();
        body = projectileObject.GetComponent<Rigidbody2D>();

        // Prevent unrelated collision damage during these tests.
        SetField("damageEnabled", false);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (projectileObject != null)
            Object.Destroy(projectileObject);

        Time.timeScale = previousTimeScale;

        // Allow Unity to finish destroying the test object.
        yield return null;
    }

    // Configure private Inspector fields without changing production code.
    private void SetField(string fieldName, object value)
    {
        FieldInfo field = typeof(BossProjectile).GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(
            field,
            Is.Not.Null,
            $"BossProjectile field '{fieldName}' was not found.");

        field.SetValue(projectile, value);
    }

    [Test]
    public void Awake_ConfiguresProjectilePhysics()
    {
        CircleCollider2D collider =
            projectileObject.GetComponent<CircleCollider2D>();

        Assert.That(body.bodyType, Is.EqualTo(RigidbodyType2D.Dynamic));
        Assert.That(body.gravityScale, Is.EqualTo(0f));
        Assert.That(body.linearDamping, Is.EqualTo(0f));
        Assert.That(
            body.constraints,
            Is.EqualTo(RigidbodyConstraints2D.FreezeRotation));

        Assert.That(collider.isTrigger, Is.True);
    }

    [UnityTest]
    public IEnumerator Initialize_NormalizesDirection_AndUsesConfiguredSpeed()
    {
        SetField("speed", 5f);

        // Length is 5, but projectile speed must still be 5—not 25.
        projectile.Initialize(new Vector2(3f, 4f));

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(body.linearVelocity.x, Is.EqualTo(3f).Within(0.01f));
        Assert.That(body.linearVelocity.y, Is.EqualTo(4f).Within(0.01f));
        Assert.That(
            body.linearVelocity.magnitude,
            Is.EqualTo(5f).Within(0.01f));

        // The sprite should face the same direction as its travel.
        float expectedAngle =
            Mathf.Atan2(4f, 3f) * Mathf.Rad2Deg;

        Assert.That(
            Mathf.Abs(Mathf.DeltaAngle(body.rotation, expectedAngle)),
            Is.LessThan(0.1f));
    }

    [UnityTest]
    public IEnumerator Initialize_ZeroDirection_FallsBackToRight()
    {
        SetField("speed", 5f);
        projectile.Initialize(Vector2.zero);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(body.linearVelocity.x, Is.EqualTo(5f).Within(0.01f));
        Assert.That(body.linearVelocity.y, Is.EqualTo(0f).Within(0.01f));

        Assert.That(
            Mathf.Abs(Mathf.DeltaAngle(body.rotation, 0f)),
            Is.LessThan(0.1f));
    }

    [UnityTest]
    public IEnumerator MovementDisabled_StopsAnAlreadyMovingProjectile()
    {
        projectile.Initialize(Vector2.right);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(body.linearVelocity.magnitude, Is.GreaterThan(0f));

        SetField("movementEnabled", false);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(
            body.linearVelocity.magnitude,
            Is.EqualTo(0f).Within(0.001f));
    }

    [UnityTest]
    public IEnumerator AutoExpire_DestroysProjectileAfterLifetime()
    {
        SetField("movementEnabled", false);
        SetField("autoExpire", true);
        SetField("lifetime", 0.05f);

        yield return new WaitForSeconds(0.15f);
        yield return null;

        Assert.That(
            projectileObject == null,
            Is.True,
            "The projectile should be destroyed after its lifetime.");
    }

    [UnityTest]
    public IEnumerator AutoExpireDisabled_PreservesProjectilePastLifetime()
    {
        SetField("movementEnabled", false);
        SetField("autoExpire", false);
        SetField("lifetime", 0.05f);

        yield return new WaitForSeconds(0.15f);
        yield return null;

        Assert.That(
            projectileObject != null,
            Is.True,
            "The projectile should remain when auto-expiration is disabled.");
    }
}