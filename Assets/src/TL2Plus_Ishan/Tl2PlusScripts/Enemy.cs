
using UnityEngine;

public class Enemy
{
    private int enemyId;
    private int hp;
    private int maxHp;
    private EnemyState currentState;

    public int Hp => hp;

    public Enemy(int id, int maxHp, EnemyState startState)
    {
        enemyId = id;
        this.maxHp = maxHp;
        hp = maxHp;
        currentState = startState;
    }

    public string Update()
    {
        return currentState.Handle(this);
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    public void TakeDamage(int amount)
    {
        if (IsDefeated() || amount <= 0)
            return;

        hp = Mathf.Max(0, hp - amount);
        Debug.Log("Enemy HP: " + hp);

        if (IsDefeated())
            ChangeState(new DeadState());
    }

    public bool IsDefeated()
    {
        return hp <= 0;
    }
}
