using UnityEngine;

public class PlayerStats
{
    public int maxHP;
    public int currentHP;

    public float moveSpeed;
    public int attackPower;
    public int defense;

    // crit ranges from 0 to 1 (0.1 is 10% etc), multiplier scales dmg on a crit
    public float critChance;
    public float critMulti;

    public PlayerStats(int maxHP = 100, float moveSpeed = 5f, int attackPower = 10,
        float critChance = 0.05f, float critMulti = 2f, int defense = 0)
    {
        this.maxHP = maxHP;
        currentHP = maxHP;
        this.moveSpeed = moveSpeed;
        this.attackPower = attackPower;
        this.critChance = critChance;
        this.critMulti = critMulti;
        this.defense = defense;
    }

    // - all the hp stuff -
    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead())
            return;

        // for now defense just directly deducts the damage recieved. cant lower dmg lower than 1
        int finalDamage = Mathf.Max(1, amount - defense);
        currentHP = Mathf.Max(0, currentHP - finalDamage);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead())
            return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    // -buffs and nerfs-
    // delta is amount of change
    public void ModifyMaxHP(int delta)
    {
        maxHP += delta;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public void ModifyMoveSpeed(float delta)
    {
        moveSpeed += delta;
    }

    public void ModifyAttackPower(int delta)
    {
        attackPower += delta;
    }

    public void ModifyDefense(int delta)
    {
        defense += delta;
    }

    public void ModifyCritChance(float delta)
    {
        critChance += delta;
    }

    // - dmg stuff -
    // returns dmg for one attack also rolling for a crit
    public int RollAttackDamage(out bool wasCrit)
    {
        wasCrit = Random.value < critChance;
        float damage = wasCrit ? attackPower * critMulti : attackPower;
        return Mathf.RoundToInt(damage);
    }
}