using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TutorialPrompt : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string message = "Presiona C para hacer Dash";
    [SerializeField] private bool showOnlyOnce = true;

    private bool hasShown;

    private void Awake()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasShown && showOnlyOnce)
        {
            return;
        }

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null)
        {
            return;
        }

        hasShown = true;
        MedusaAbilityUI.ShowTemporaryMessage(message);
    }
}
