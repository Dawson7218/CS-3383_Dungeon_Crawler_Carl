using UnityEngine;

public class BindingDemo : MonoBehaviour
{
    Enemy enemy = new Enemy(1, 100, new IdleState());

    // actual objects the declared type EnemyState can hold
    EnemyState[] states = { new IdleState(), new ChaseState(), new AttackState(), new DeadState() };
    int index = 0;

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), enemy.Update());

        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
        {
            index = (index + 1) % states.Length;
            enemy.ChangeState(states[index]);
        }
    }
}