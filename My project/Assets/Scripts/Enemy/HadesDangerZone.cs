using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HadesDangerZone : MonoBehaviour
{
    private int damage = 1;
    private float warningTime = 0.6f;
    private float activeTime = 0.8f;
    private Collider2D damageCollider;
    private SpriteRenderer spriteRenderer;
    private bool isActive;
    private bool hasDamagedPlayer;

    private void Awake()
    {
        damageCollider = GetComponent<Collider2D>();
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 30;
        }
    }

    public void Initialize(int zoneDamage, float warningDuration, float activeDuration)
    {
        damage = zoneDamage;
        warningTime = warningDuration;
        activeTime = activeDuration;
        StartCoroutine(ZoneRoutine());
    }

    private IEnumerator ZoneRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.85f, 0.1f, 0.45f);
        }

        yield return new WaitForSeconds(warningTime);

        isActive = true;
        damageCollider.enabled = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.15f, 0.05f, 0.75f);
        }

        yield return new WaitForSeconds(activeTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamage(collision);
    }

    private void TryDamage(Collider2D collision)
    {
        if (!isActive || hasDamagedPlayer)
        {
            return;
        }

        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();
        if (player == null)
        {
            return;
        }

        hasDamagedPlayer = true;
        Vector2 knockbackDirection = player.transform.position - transform.position;
        player.TakeDamage(damage, knockbackDirection);
    }
}
