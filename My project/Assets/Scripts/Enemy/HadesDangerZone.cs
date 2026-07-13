using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HadesDangerZone : MonoBehaviour
{
    private static readonly Color WarningColor = new Color(0.8f, 0.18f, 1f, 0.28f);
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
        if (spriteRenderer != null)
        {
            spriteRenderer.color = WarningColor;
        }

        yield return new WaitForSeconds(warningTime);

        isActive = true;
        damageCollider.enabled = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.72f, 0.05f, 1f, 0.42f);
        }

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
            line.useWorldSpace = false;
            line.positionCount = 8;
            line.startWidth = i == 0 ? 0.075f : 0.035f;
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

        for (int boltIndex = 0; boltIndex < lightningBolts.Length; boltIndex++)
        {
            LineRenderer line = lightningBolts[boltIndex];
            float baseX = (boltIndex - 1) * 0.22f;

            for (int pointIndex = 0; pointIndex < line.positionCount; pointIndex++)
            {
                float progress = pointIndex / (float)(line.positionCount - 1);
                float jitter = pointIndex == 0 || pointIndex == line.positionCount - 1
                    ? 0f
                    : Random.Range(-0.16f, 0.16f);
                line.SetPosition(pointIndex, new Vector3(baseX + jitter, 0.52f - progress * 1.04f, -0.02f));
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
