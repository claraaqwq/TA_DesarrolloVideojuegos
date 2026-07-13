using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Vidas")]
    public int maxLives = 3;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 0.25f;

    [Header("Eventos")]
    public UnityEvent OnGameOver;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private CharacterController2D characterController;
    private Animator animator;
    private bool isDead;
    private int currentLives;

    public int CurrentLives => currentLives;

    private void Start()
    {
        currentHealth = maxHealth;
        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        characterController = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

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
            currentLives--;

            if (currentLives > 0)
            {
                StartCoroutine(DeathAndRespawn());
            }
            else
            {
                StartCoroutine(FinalDeath());
            }
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
        {
            return;
        }

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

    private IEnumerator DeathAndRespawn()
    {
        isDead = true;
        Debug.Log("El jugador murio");

        if (characterController != null)
        {
            characterController.SetMovementEnabled(false);
        }

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(respawnDelay);

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        currentHealth = maxHealth;

        if (animator != null)
        {
            animator.SetBool("IsDead", false);
            animator.SetBool("Hit", false);
        }

        if (characterController != null)
        {
            characterController.SetMovementEnabled(true);
        }

        isDead = false;
    }

    private IEnumerator FinalDeath()
    {
        isDead = true;
        Debug.Log("Game Over: se acabaron las vidas.");

        if (characterController != null)
        {
            characterController.SetMovementEnabled(false);
        }

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(respawnDelay);

        OnGameOver?.Invoke();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    private IEnumerator DamageFeedback()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator HealFeedback()
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }
}
