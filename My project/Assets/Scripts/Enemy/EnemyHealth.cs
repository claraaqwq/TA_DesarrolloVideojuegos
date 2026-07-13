using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 12;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Knockback")]
    [SerializeField] private float knockbackDistance = 1f;
    [SerializeField] private float knockbackDuration = 0.3f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private bool isDead;

    public event Action<int, int> HealthChanged;
    public event Action Died;
    public event Action<Vector2, float, float> KnockedBack;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection = default)
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

        if (knockbackDirection != Vector2.zero)
        {
            float horizontalSign = knockbackDirection.x >= 0f ? 1f : -1f;
            Vector2 horizontalKnockback = new Vector2(horizontalSign, 0f);
            KnockedBack?.Invoke(horizontalKnockback, knockbackDistance, knockbackDuration);
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
