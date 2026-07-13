using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HadesDangerZone : MonoBehaviour
{
    private static readonly Color LightningColor = new Color(0.92f, 0.72f, 1f, 1f);

    private int damage = 1;
    private float warningTime = 0.6f;
    private float activeTime = 0.8f;
    private Collider2D damageCollider;
    private SpriteRenderer spriteRenderer;
    private LineRenderer[] lightningBolts;
    private float nextFlickerTime;
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
            // El propio rayo tenue sirve como advertencia. Ocultamos el
            // rectángulo para no cubrir el escenario con una mancha morada.
            spriteRenderer.enabled = false;
        }

        CreateLightningVisuals();
    }

    private void Update()
    {
        if (Time.time < nextFlickerTime)
        {
            return;
        }

        nextFlickerTime = Time.time + (isActive ? 0.045f : 0.1f);
        RefreshLightning();
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
        yield return new WaitForSeconds(warningTime);

        isActive = true;
        damageCollider.enabled = true;

        yield return new WaitForSeconds(activeTime);
        Destroy(gameObject);
    }

    private void CreateLightningVisuals()
    {
        lightningBolts = new LineRenderer[3];

        for (int i = 0; i < lightningBolts.Length; i++)
        {
            GameObject boltObject = new GameObject("LightningBolt_" + i);
            boltObject.transform.SetParent(transform, false);

            LineRenderer line = boltObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 12;
            line.startWidth = i == 0 ? 0.18f : 0.085f;
            line.endWidth = line.startWidth * 0.55f;
            line.numCornerVertices = 2;
            line.numCapVertices = 2;
            line.sortingLayerName = "Default";
            line.sortingOrder = 32 + i;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = i == 0 ? LightningColor : new Color(0.65f, 0.12f, 1f, 0.8f);
            line.endColor = new Color(1f, 0.88f, 1f, 0.9f);
            lightningBolts[i] = line;
        }

        RefreshLightning();
    }

    private void RefreshLightning()
    {
        if (lightningBolts == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        float topY = mainCamera != null
            ? mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1.08f, mainCamera.nearClipPlane)).y
            : transform.position.y + 6f;
        float bottomY = transform.position.y - Mathf.Abs(transform.lossyScale.y) * 0.5f;
        float zoneWidth = Mathf.Abs(transform.lossyScale.x);

        for (int boltIndex = 0; boltIndex < lightningBolts.Length; boltIndex++)
        {
            LineRenderer line = lightningBolts[boltIndex];
            float baseX = transform.position.x + (boltIndex - 1) * zoneWidth * 0.22f;

            for (int pointIndex = 0; pointIndex < line.positionCount; pointIndex++)
            {
                float progress = pointIndex / (float)(line.positionCount - 1);
                float jitter = pointIndex == 0 || pointIndex == line.positionCount - 1
                    ? 0f
                    : Random.Range(-zoneWidth * 0.14f, zoneWidth * 0.14f);
                float y = Mathf.Lerp(topY, bottomY, progress);
                line.SetPosition(pointIndex, new Vector3(baseX + jitter, y, transform.position.z - 0.02f));
            }

            float alpha = isActive ? Random.Range(0.75f, 1f) : Random.Range(0.18f, 0.42f);
            Color start = line.startColor;
            Color end = line.endColor;
            start.a = alpha;
            end.a = alpha;
            line.startColor = start;
            line.endColor = end;
        }
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
