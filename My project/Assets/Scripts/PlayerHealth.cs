using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Respawn")]
    public Transform respawnPoint;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Vida actual: " + currentHealth);

        if (spriteRenderer != null)
        {
            StartCoroutine(DamageFeedback());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Vida actual: " + currentHealth);

        if (spriteRenderer != null)
        {
            StartCoroutine(HealFeedback());
        }
    }

    private void Die()
    {
        Debug.Log("El jugador murió");

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            currentHealth = maxHealth;
        }
    }

    private System.Collections.IEnumerator DamageFeedback()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    private System.Collections.IEnumerator HealFeedback()
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }
}