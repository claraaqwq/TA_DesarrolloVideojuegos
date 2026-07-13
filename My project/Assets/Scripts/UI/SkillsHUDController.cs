using UnityEngine;
using UnityEngine.UI;

public class SkillsHUDController : MonoBehaviour
{
    [Header("Referencias del jugador")]
    [SerializeField] private CharacterController2D playerController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerMedusaAbility medusaAbility;
    [SerializeField] private PlayerAriadnaThread ariadnaThread;

    [Header("Overlays de cooldown (hijos 'Filled' de cada icono)")]
    [SerializeField] private Image dashCooldownOverlay;
    [SerializeField] private Image blinkCooldownOverlay;
    [SerializeField] private Image medusaCooldownOverlay;
    [SerializeField] private Image ariadnaCooldownOverlay;

    private void Update()
    {
        UpdateOverlay(dashCooldownOverlay, playerController != null ? playerController.DashCooldownRemaining : 0f, playerController != null ? playerController.DashCooldownDuration : 0f);
        UpdateOverlay(blinkCooldownOverlay, playerMovement != null ? playerMovement.BlinkCooldownRemaining : 0f, playerMovement != null ? playerMovement.BlinkCooldownDuration : 0f);
        UpdateOverlay(ariadnaCooldownOverlay, ariadnaThread != null ? ariadnaThread.CooldownRemaining : 0f, ariadnaThread != null ? ariadnaThread.CooldownDuration : 0f);

        if (medusaCooldownOverlay != null)
        {
            bool showMedusa = medusaAbility != null && GameProgress.MedusaUnlocked;
            medusaCooldownOverlay.transform.parent.gameObject.SetActive(showMedusa);

            if (showMedusa)
            {
                UpdateOverlay(medusaCooldownOverlay, medusaAbility.CooldownRemaining, medusaAbility.CooldownDuration);
            }
        }
    }

    private static void UpdateOverlay(Image overlay, float remaining, float total)
    {
        if (overlay == null || total <= 0f)
        {
            return;
        }

        overlay.fillAmount = Mathf.Clamp01(remaining / total);
    }
}
