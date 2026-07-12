using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private HadesBossController bossController;
    [SerializeField] private BossArenaGate bossArenaGate;

    private bool triggered;

    private void Awake()
    {
        BoxCollider2D triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
        {
            return;
        }

        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();
        if (player == null)
        {
            return;
        }

        triggered = true;

        if (bossController != null)
        {
            bossController.StartCombat();
        }

        if (bossArenaGate != null)
        {
            bossArenaGate.Close();
        }

        gameObject.SetActive(false);
    }
}
