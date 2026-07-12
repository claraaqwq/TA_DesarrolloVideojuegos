using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class BossScenePortal : MonoBehaviour
{
    [SerializeField] private string bossSceneName = "BossFinal";

    private bool isLoading;

    private void Awake()
    {
        BoxCollider2D portalCollider = GetComponent<BoxCollider2D>();
        portalCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryLoadBossScene(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryLoadBossScene(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryLoadBossScene(collision.collider);
    }

    private void TryLoadBossScene(Collider2D other)
    {
        if (isLoading)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(bossSceneName))
        {
            Debug.LogError($"BossScenePortal no puede cargar la escena '{bossSceneName}'. Revisa Build Settings.");
            return;
        }

        isLoading = true;
        Debug.Log($"Player-Hermes entro al portal. Cargando escena '{bossSceneName}'.");
        SceneManager.LoadScene(bossSceneName);
    }
}
