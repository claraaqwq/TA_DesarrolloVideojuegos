using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyHealth bossHealth;
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        if (bossHealth != null)
        {
            bossHealth.HealthChanged += UpdateBar;
            bossHealth.Died += Hide;
            UpdateBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        }
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.HealthChanged -= UpdateBar;
            bossHealth.Died -= Hide;
        }
    }

    public void SetBoss(EnemyHealth health)
    {
        if (bossHealth != null)
        {
            bossHealth.HealthChanged -= UpdateBar;
            bossHealth.Died -= Hide;
        }

        bossHealth = health;

        if (bossHealth != null)
        {
            bossHealth.HealthChanged += UpdateBar;
            bossHealth.Died += Hide;
            UpdateBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        }
    }

    private void UpdateBar(int currentHealth, int maxHealth)
    {
        Debug.Log($"[BossHealthBar] UpdateBar llamado: {currentHealth}/{maxHealth}");

        if (fillImage == null || maxHealth <= 0)
        {
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
