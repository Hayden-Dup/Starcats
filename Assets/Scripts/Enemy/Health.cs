// Assets/Scripts/Health.cs
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // you could play a death animation here
        Destroy(gameObject);
    }
}
