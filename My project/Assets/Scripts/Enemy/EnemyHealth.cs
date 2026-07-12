using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 12;
    [SerializeField] private bool destroyOnDeath = true;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private bool isDead;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || isDead)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{name} recibio {damage} de dano. Vida: {currentHealth}");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), 0.12f);
        }

        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{name} murio");
        Died?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
