using UnityEngine;

public class Enemy
{
    int enemyId;
    int hp;
    int maxHp;
    EnemyState currentState; // declared: EnemyState · actual: whichever concrete state

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
        return currentState.Handle(this); // dynamic binding happens here
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    public void TakeDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);
        if (IsDefeated()) ChangeState(new DeadState());
    }

    public bool IsDefeated()
    {
        return hp <= 0;
    }
}