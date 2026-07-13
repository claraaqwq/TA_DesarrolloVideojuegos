using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;
    public int damage = 1;

    private Vector3 startPosition;
    private int direction = 1;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;
    private bool isKnockedBack;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.KnockedBack += HandleKnockback;
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.KnockedBack -= HandleKnockback;
        }
    }

    private void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            return;
        }

        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        float distanceFromStart = transform.position.x - startPosition.x;

        if (direction == 1 && distanceFromStart >= moveDistance)
        {
            direction = -1;
            FlipSprite();
        }
        else if (direction == -1 && distanceFromStart <= -moveDistance)
        {
            direction = 1;
            FlipSprite();
        }
    }

    private void HandleKnockback(Vector2 knockDirection, float distance, float duration)
    {
        StartCoroutine(KnockbackRoutine(knockDirection, distance, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 knockDirection, float distance, float duration)
    {
        isKnockedBack = true;

        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(knockDirection.normalized * distance);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(elapsed / duration), 3f);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        isKnockedBack = false;
    }

    private void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction == -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            Vector2 knockbackDirection = player.transform.position - transform.position;
            player.TakeDamage(damage, knockbackDirection);
        }
    }
}