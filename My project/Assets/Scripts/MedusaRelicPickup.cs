using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MedusaRelicPickup : MonoBehaviour
{
    private bool collected;

    private void Awake()
    {
        Collider2D pickupCollider = GetComponent<Collider2D>();
        pickupCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        collected = true;
        GameProgress.UnlockMedusa();
        MedusaAbilityUI.ShowUnlockMessage();
        Debug.Log("Mirada de Piedra desbloqueada.");
        Destroy(gameObject);
    }
}
