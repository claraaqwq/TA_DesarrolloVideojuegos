using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Lever : MonoBehaviour
{
    [SerializeField] private Seal[] sealsToDeactivate;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color activatedColor = new Color(1f, 0.85f, 0.3f, 1f);

    private bool activated;

    private void Awake()
    {
        Collider2D leverCollider = GetComponent<Collider2D>();
        leverCollider.isTrigger = true;

        if (visual == null)
        {
            visual = GetComponent<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated)
        {
            return;
        }

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null)
        {
            return;
        }

        activated = true;

        foreach (Seal seal in sealsToDeactivate)
        {
            if (seal != null)
            {
                seal.Deactivate();
            }
        }

        if (visual != null)
        {
            visual.color = activatedColor;
        }

        MedusaAbilityUI.ShowTemporaryMessage("La palanca cede con un crujido...");
    }
}
