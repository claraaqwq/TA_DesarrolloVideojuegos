using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Feedback visual (opcional)")]
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private Color activeColor = new Color(0.4f, 1f, 0.6f, 1f);

    private bool activated;

    private void Awake()
    {
        Collider2D checkpointCollider = GetComponent<Collider2D>();
        checkpointCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        activated = true;
        playerHealth.SetRespawnPoint(transform);
        MedusaAbilityUI.ShowTemporaryMessage("El hilo se ancla en este punto...");

        if (indicator != null)
        {
            indicator.color = activeColor;
        }
    }
}
