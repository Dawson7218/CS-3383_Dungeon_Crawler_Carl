using UnityEngine;

// Drone. Stops at attackRange and shoots a laser instead of hitting.
public class RangedEnemyController : EnemyController
{
    [Header("Ranged Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override string IdleAnim => "Drone_Idle";
    protected override string MoveAnim => "Drone_Move";
    protected override string AttackAnim => "Drone_Attack";
    protected override string DeathAnim => "Drone_Death";
    protected override float AttackAnimDuration => 0.5f;

    protected override void PerformAttack()
    {
        if (projectilePrefab == null || player == null)
            return;

        Vector3 spawnPosition = GetFirePosition();

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        DroneProjectile laser =
            projectile.GetComponent<DroneProjectile>();

        if (laser != null)
        {
            Vector2 direction =
                ((Vector2)player.position - (Vector2)spawnPosition).normalized;

            laser.Initialize(direction);
        }
    }

    // flipX only flips the sprite, not the child objects, so the fire
    // point has to be mirrored by hand when the drone faces the other way.
    private Vector3 GetFirePosition()
    {
        if (firePoint == null)
            return transform.position;

        Vector3 local = transform.InverseTransformPoint(firePoint.position);

        if (FacingRight != spriteFacesRightByDefault)
            local.x = -local.x;

        return transform.TransformPoint(local);
    }
}
