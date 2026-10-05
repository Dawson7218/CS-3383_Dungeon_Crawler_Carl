public class DeadState : EnemyState
{
    public string Handle(Enemy enemy) { return "Dead: no more actions"; }
}