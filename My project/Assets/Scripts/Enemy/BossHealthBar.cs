using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyHealth bossHealth;
    [SerializeField] private Image fillImage;

    private bool isSubscribed;
    private static Sprite fallbackFillSprite;

    private void Awake()
    {
        FindFillImageIfNeeded();
    }

    private void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void SetBoss(EnemyHealth health)
    {
        Unsubscribe();

        bossHealth = health;
        FindFillImageIfNeeded();

        if (isActiveAndEnabled)
        {
            Subscribe();
        }

        Refresh();
    }

    private void Subscribe()
    {
        if (bossHealth == null || isSubscribed)
        {
            return;
        }

        bossHealth.HealthChanged += UpdateBar;
        bossHealth.Died += Hide;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (bossHealth == null || !isSubscribed)
        {
            isSubscribed = false;
            return;
        }

        bossHealth.HealthChanged -= UpdateBar;
        bossHealth.Died -= Hide;
        isSubscribed = false;
    }

    private void Refresh()
    {
        if (bossHealth != null)
        {
            UpdateBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        }
    }

    private void FindFillImageIfNeeded()
    {
        if (fillImage == null)
        {
            fillImage = GetComponentInChildren<Image>(true);
        }
    }

    private void UpdateBar(int currentHealth, int maxHealth)
    {
        if (fillImage == null || maxHealth <= 0)
        {
            return;
        }

        EnsureFillSprite();

        if (fillImage.type != Image.Type.Filled)
        {
            fillImage.type = Image.Type.Filled;
        }

        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
    }

    private void EnsureFillSprite()
    {
        if (fillImage.sprite != null)
        {
            return;
        }

        if (fallbackFillSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            fallbackFillSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                1f);
            fallbackFillSprite.name = "BossHealthBarFill";
        }

        fillImage.sprite = fallbackFillSprite;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
