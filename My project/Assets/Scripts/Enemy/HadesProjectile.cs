using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class HadesProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 4f;

    private int damage = 1;
    private Rigidbody2D rb;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;
    }

    public void Initialize(Vector2 direction, float speed, int projectileDamage, float maxLifetime)
    {
        damage = projectileDamage;
        lifetime = maxLifetime;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
        {
            return;
        }

        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();
        if (player == null)
        {
            return;
        }

        hasHit = true;
        player.TakeDamage(damage, rb.linearVelocity);
        Destroy(gameObject);
    }
}
