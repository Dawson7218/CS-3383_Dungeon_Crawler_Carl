using UnityEngine;

public class PlayerStats
{
    public int maxHP;
    public int currentHP;

    public PlayerStats(int maxHP = 100)
    {
        this.maxHP = maxHP;
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead())
            return;

        currentHP = Mathf.Max(0, currentHP - amount);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}