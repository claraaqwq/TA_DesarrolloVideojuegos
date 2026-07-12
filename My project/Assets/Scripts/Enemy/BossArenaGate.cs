using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossArenaGate : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gateVisual;

    private BoxCollider2D gateCollider;

    private void Awake()
    {
        gateCollider = GetComponent<BoxCollider2D>();
        Open();
    }

    public void Close()
    {
        if (gateCollider != null)
        {
            gateCollider.enabled = true;
        }

        if (gateVisual != null)
        {
            gateVisual.enabled = true;
        }
    }

    public void Open()
    {
        if (gateCollider != null)
        {
            gateCollider.enabled = false;
        }

        if (gateVisual != null)
        {
            gateVisual.enabled = false;
        }
    }
}
