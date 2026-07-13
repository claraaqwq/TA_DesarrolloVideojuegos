using System.Collections;
using UnityEngine;

public class PlayerMedusaAbility : MonoBehaviour
{
    [Header("Mirada de Piedra")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float boxHeight = 2.5f;
    [SerializeField] private float petrifyDuration = 2f;
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private LayerMask enemyLayer;

    private float cooldownRemaining;
    private SpriteRenderer spriteRenderer;

    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownRemaining;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyLayer.value == 0)
        {
            enemyLayer = LayerMask.GetMask("Enemy");
        }
    }

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryUseMedusa();
        }
    }

    private void TryUseMedusa()
    {
        if (!GameProgress.MedusaUnlocked)
        {
            MedusaAbilityUI.ShowTemporaryMessage("Mirada de Piedra bloqueada");
            return;
        }

        if (cooldownRemaining > 0f)
        {
            MedusaAbilityUI.ShowTemporaryMessage($"Mirada de Piedra en recarga: {Mathf.CeilToInt(cooldownRemaining)}s");
            return;
        }

        int direction = GetFacingDirection();
        Vector2 center = (Vector2)transform.position + Vector2.right * direction * (range * 0.5f);
        Vector2 size = new Vector2(range, boxHeight);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, enemyLayer);

        bool petrifiedAnyTarget = false;
        foreach (Collider2D hit in hits)
        {
            if (hit.transform.IsChildOf(transform))
            {
                continue;
            }

            HadesBossController hades = hit.GetComponentInParent<HadesBossController>();
            if (hades != null)
            {
                hades.Petrify(petrifyDuration);
                petrifiedAnyTarget = true;
            }
        }

        cooldownRemaining = cooldown;
        StartCoroutine(CastFeedback());

        if (!petrifiedAnyTarget)
        {
            MedusaAbilityUI.ShowTemporaryMessage("No hay objetivo para petrificar");
        }
    }

    private int GetFacingDirection()
    {
        if (Mathf.Abs(transform.localScale.x) > 0.01f)
        {
            return transform.localScale.x >= 0f ? 1 : -1;
        }

        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            return -1;
        }

        return 1;
    }

    private IEnumerator CastFeedback()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(0.5f, 1f, 0.55f, 1f);
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
    }

    private void OnDrawGizmosSelected()
    {
        int direction = transform.localScale.x >= 0f ? 1 : -1;
        Vector2 center = (Vector2)transform.position + Vector2.right * direction * (range * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, new Vector3(range, boxHeight, 0f));
    }
}
