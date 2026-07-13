using UnityEngine;
using System.Collections;

public class DamageZone : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;

    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();

        if (player != null && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageLoop(player));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();

        if (player != null && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator DamageLoop(PlayerHealth player)
    {
        while (true)
        {
            Vector2 knockbackDirection = player.transform.position - transform.position;
            player.TakeDamage(damage, knockbackDirection);
            yield return new WaitForSeconds(damageCooldown);
        }
    }
}