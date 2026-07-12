using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EnemyHealth))]
public class HadesBossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private CharacterController2D playerController;
    [SerializeField] private PlayerAttack playerAttack;

    [Header("Ataque 1 - Proyectil")]
    [SerializeField] private float projectileCooldown = 2f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float projectileLifetime = 4f;

    [Header("Ataque 2 - Zona de peligro")]
    [SerializeField] private float dangerZoneCooldown = 2.5f;
    [SerializeField] private float warningTime = 0.65f;
    [SerializeField] private float activeTime = 0.8f;
    [SerializeField] private int dangerZoneDamage = 1;
    [SerializeField] private Vector2 dangerZoneSize = new Vector2(1.4f, 2.5f);

    private EnemyHealth enemyHealth;
    private bool defeated;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.Died += HandleDefeat;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerController = playerObject.GetComponent<CharacterController2D>();
                playerAttack = playerObject.GetComponent<PlayerAttack>();
            }
        }

        StartCoroutine(AttackLoop());
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= HandleDefeat;
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!defeated)
        {
            FireProjectile();
            yield return new WaitForSeconds(projectileCooldown);

            if (defeated)
            {
                yield break;
            }

            SpawnDangerZone();
            yield return new WaitForSeconds(dangerZoneCooldown);
        }
    }

    private void FireProjectile()
    {
        if (player == null)
        {
            return;
        }

        GameObject projectile = new GameObject("Hades_Projectile");
        projectile.transform.position = transform.position + Vector3.up * 0.2f;
        projectile.layer = gameObject.layer;

        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        renderer.color = new Color(0.8f, 0.1f, 1f, 1f);
        renderer.sortingOrder = 20;

        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.radius = 0.25f;

        Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        HadesProjectile hadesProjectile = projectile.AddComponent<HadesProjectile>();
        Vector2 direction = player.position - projectile.transform.position;
        hadesProjectile.Initialize(direction, projectileSpeed, projectileDamage, projectileLifetime);
    }

    private void SpawnDangerZone()
    {
        if (player == null)
        {
            return;
        }

        GameObject zone = new GameObject("Hades_DangerZone");
        zone.transform.position = new Vector3(player.position.x, player.position.y, 0f);

        SpriteRenderer renderer = zone.AddComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 0.85f, 0.1f, 0.45f);
        renderer.sortingOrder = 19;

        BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
        collider.size = dangerZoneSize;

        HadesDangerZone dangerZone = zone.AddComponent<HadesDangerZone>();
        dangerZone.Initialize(dangerZoneDamage, warningTime, activeTime);
    }

    private void HandleDefeat()
    {
        defeated = true;
        StopAllCoroutines();

        Collider2D bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }

        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
        }

        if (playerAttack != null)
        {
            playerAttack.enabled = false;
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Debug.Log("Victoria: Hades fue derrotado.");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
