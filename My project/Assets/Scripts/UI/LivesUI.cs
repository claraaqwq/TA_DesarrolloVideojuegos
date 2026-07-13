using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    [Header("Referencia al jugador")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Texto")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private string label = "Vidas: ";

    private void Update()
    {
        if (playerHealth == null || livesText == null)
        {
            return;
        }

        livesText.text = label + playerHealth.CurrentLives;
    }
}
