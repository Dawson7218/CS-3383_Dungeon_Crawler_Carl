using UnityEngine;

// Fast melee enemy. Same behaviour as the base class, just its own clips.
public class GruntEnemyController : EnemyController
{
    protected override string IdleAnim => "gruntIdle";
    protected override string MoveAnim => "GruntRun";
    protected override string AttackAnim => "gruntAttack";
    protected override string DeathAnim => "GruntDie";

    protected override void PerformAttack()
    {
        DamagePlayer(attackDamage);
    }
}
