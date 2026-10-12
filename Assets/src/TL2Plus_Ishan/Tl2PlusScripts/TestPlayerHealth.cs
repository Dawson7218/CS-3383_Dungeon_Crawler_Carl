
using UnityEngine;

public class TestPlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public int CurrentHP => currentHealth;
    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log("Player HP: " + currentHealth);

        if (IsDead)
        {
            Debug.Log("Player defeated!");
        }
    }
}
