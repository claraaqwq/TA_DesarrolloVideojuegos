using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Referencia al jugador")]
    public PlayerHealth playerHealth;

    [Header("Corazones")]
    public Image[] hearts;

    [Header("Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Update()
    {
        if (playerHealth == null)
        {
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                hearts[i].sprite = fullHeart;
                hearts[i].enabled = true;
            }
            else
            {
                if (emptyHeart != null)
                {
                    hearts[i].sprite = emptyHeart;
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }
        }
    }
}