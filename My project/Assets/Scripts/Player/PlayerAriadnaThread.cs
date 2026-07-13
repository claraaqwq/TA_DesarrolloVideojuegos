using UnityEngine;

public class PlayerAriadnaThread : MonoBehaviour
{
    [Header("Hilo de Ariadna")]
    [SerializeField] private float anchorWindow = 6f;
    [SerializeField] private float cooldown = 8f;
    [SerializeField] private Sprite anchorMarkerSprite;

    private bool hasAnchor;
    private Vector3 anchorPosition;
    private int anchorHealth;
    private float anchorTimeRemaining;
    private float cooldownRemaining;
    private GameObject anchorMarker;

    private PlayerHealth playerHealth;
    private Rigidbody2D rb;

    private static Sprite fallbackAnchorSprite;

    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownRemaining;
    public bool HasActiveAnchor => hasAnchor;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
        }

        if (hasAnchor)
        {
            anchorTimeRemaining -= Time.deltaTime;

            if (anchorTimeRemaining <= 0f)
            {
                MedusaAbilityUI.ShowTemporaryMessage("El Hilo de Ariadna se desvaneció");
                ClearAnchor();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryUseThread();
        }
    }

    private void TryUseThread()
    {
        if (hasAnchor)
        {
            Rewind();
            return;
        }

        if (cooldownRemaining > 0f)
        {
            MedusaAbilityUI.ShowTemporaryMessage($"Hilo de Ariadna en recarga: {Mathf.CeilToInt(cooldownRemaining)}s");
            return;
        }

        PlaceAnchor();
    }

    private void PlaceAnchor()
    {
        hasAnchor = true;
        anchorPosition = transform.position;
        anchorHealth = playerHealth != null ? playerHealth.currentHealth : 0;
        anchorTimeRemaining = anchorWindow;

        anchorMarker = CreateAnchorMarker(anchorPosition);
        MedusaAbilityUI.ShowTemporaryMessage("Hilo de Ariadna colocado - Q para regresar");
    }

    private void Rewind()
    {
        transform.position = anchorPosition;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (playerHealth != null && anchorHealth > playerHealth.currentHealth)
        {
            playerHealth.Heal(anchorHealth - playerHealth.currentHealth);
        }

        ClearAnchor();
        cooldownRemaining = cooldown;
        MedusaAbilityUI.ShowTemporaryMessage("Hilo de Ariadna: regresaste en el tiempo");
    }

    private void ClearAnchor()
    {
        hasAnchor = false;

        if (anchorMarker != null)
        {
            Destroy(anchorMarker);
            anchorMarker = null;
        }
    }

    private GameObject CreateAnchorMarker(Vector3 position)
    {
        GameObject marker = new GameObject("AriadnaAnchor");
        marker.transform.position = position;

        SpriteRenderer renderer = marker.AddComponent<SpriteRenderer>();
        renderer.sprite = anchorMarkerSprite != null ? anchorMarkerSprite : GetFallbackAnchorSprite();
        renderer.color = new Color(1f, 0.85f, 0.3f, 0.9f);
        renderer.sortingOrder = 20;

        return marker;
    }

    private static Sprite GetFallbackAnchorSprite()
    {
        if (fallbackAnchorSprite != null)
        {
            return fallbackAnchorSprite;
        }

        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "AriadnaAnchorFallback";

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.45f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        fallbackAnchorSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return fallbackAnchorSprite;
    }

    private void OnDrawGizmosSelected()
    {
        if (!hasAnchor)
        {
            return;
        }

        Gizmos.color = new Color(1f, 0.85f, 0.3f, 1f);
        Gizmos.DrawWireSphere(anchorPosition, 0.4f);
    }
}
