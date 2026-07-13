using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class HadesProjectile : MonoBehaviour
{
    private static readonly Color CoreColor = new Color(1f, 0.78f, 1f, 1f);
    private static readonly Color EnergyColor = new Color(0.62f, 0.08f, 1f, 1f);

    [SerializeField] private float lifetime = 4f;

    private int damage = 1;
    private Rigidbody2D rb;
    private Transform aura;
    private TrailRenderer trail;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;

        CreateVisuals();
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, 240f * Time.deltaTime);

        if (aura != null)
        {
            float pulse = 1.7f + Mathf.Sin(Time.time * 13f) * 0.2f;
            aura.localScale = Vector3.one * pulse;
        }
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

    private void CreateVisuals()
    {
        SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();
        if (mainRenderer != null)
        {
            mainRenderer.color = CoreColor;
        }

        GameObject auraObject = new GameObject("Aura");
        auraObject.transform.SetParent(transform, false);
        aura = auraObject.transform;

        SpriteRenderer auraRenderer = auraObject.AddComponent<SpriteRenderer>();
        auraRenderer.sprite = mainRenderer != null ? mainRenderer.sprite : null;
        auraRenderer.color = new Color(EnergyColor.r, EnergyColor.g, EnergyColor.b, 0.38f);
        auraRenderer.sortingLayerName = "Default";
        auraRenderer.sortingOrder = 29;
        aura.localScale = Vector3.one * 1.7f;

        trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.28f;
        trail.minVertexDistance = 0.03f;
        trail.startWidth = 0.38f;
        trail.endWidth = 0f;
        trail.numCornerVertices = 3;
        trail.numCapVertices = 4;
        trail.sortingLayerName = "Default";
        trail.sortingOrder = 28;
        trail.material = new Material(Shader.Find("Sprites/Default"));

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(CoreColor, 0f),
                new GradientColorKey(EnergyColor, 0.45f),
                new GradientColorKey(new Color(0.18f, 0.02f, 0.35f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0.55f, 0.45f),
                new GradientAlphaKey(0f, 1f)
            });
        trail.colorGradient = gradient;
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
