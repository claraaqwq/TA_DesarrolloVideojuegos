using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyHealth))]
public class HadesBossController : MonoBehaviour
{
    private const string EffectsSortingLayer = "Default";
    private const int EffectsSortingOrder = 30;
    private const string EnemyLayerName = "Enemy";

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private CharacterController2D playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private BossArenaGate bossArenaGate;

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

    [Header("Fases del combate")]
    [SerializeField] private int bossMaxHealth = 28;
    [SerializeField, Range(0.1f, 0.9f)] private float phaseTwoHealthPercent = 0.65f;
    [SerializeField, Range(0.05f, 0.8f)] private float phaseThreeHealthPercent = 0.3f;
    [SerializeField] private float phaseTwoCooldownMultiplier = 0.72f;
    [SerializeField] private float phaseThreeCooldownMultiplier = 0.5f;
    [SerializeField] private float phaseTwoProjectileSpeedMultiplier = 1.2f;
    [SerializeField] private float phaseThreeProjectileSpeedMultiplier = 1.45f;
    [SerializeField] private float projectileBurstDelay = 0.18f;
    [SerializeField] private float finalPhaseSecondRayDelay = 0.22f;

    private EnemyHealth enemyHealth;
    private SpriteRenderer spriteRenderer;
    private Coroutine attackLoopCoroutine;
    private Coroutine petrifyCoroutine;
    private Color originalColor;
    private bool combatStarted;
    private bool defeated;
    private bool isPetrified;
    private int currentPhase = 1;
    private static Sprite projectileVisualSprite;
    private static Sprite dangerZoneVisualSprite;

    private void Awake()
    {
        ConfigureCombatPhysics();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.ConfigureMaxHealth(bossMaxHealth > 0 ? bossMaxHealth : 28);
        enemyHealth.Died += HandleDefeat;
        enemyHealth.HealthChanged += HandleHealthChanged;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        if (victoryPanel != null)
        {
            VictoryMenuController victoryMenu = victoryPanel.GetComponent<VictoryMenuController>();
            if (victoryMenu == null)
            {
                victoryPanel.AddComponent<VictoryMenuController>();
            }

            victoryPanel.SetActive(false);
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.SetActive(false);
        }

        if (bossArenaGate != null)
        {
            bossArenaGate.Open();
        }
    }

    private void ConfigureCombatPhysics()
    {
        int enemyLayer = LayerMask.NameToLayer(EnemyLayerName);
        if (enemyLayer >= 0)
        {
            gameObject.layer = enemyLayer;
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        Collider2D bossCollider = GetComponent<Collider2D>();
        if (bossCollider != null)
        {
            bossCollider.isTrigger = true;
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
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= HandleDefeat;
            enemyHealth.HealthChanged -= HandleHealthChanged;
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!defeated)
        {
            if (isPetrified)
            {
                yield return null;
                continue;
            }

            int projectilePhase = currentPhase;
            int projectileCount = projectilePhase == 1 ? 1 : projectilePhase == 2 ? 2 : 3;

            for (int i = 0; i < projectileCount; i++)
            {
                FireProjectile(GetProjectileSpeedMultiplier(projectilePhase));

                if (i < projectileCount - 1)
                {
                    yield return new WaitForSeconds(projectileBurstDelay);
                }
            }

            yield return new WaitForSeconds(projectileCooldown * GetCooldownMultiplier());

            if (defeated || isPetrified)
            {
                continue;
            }

            int rayPhase = currentPhase;
            SpawnDangerZone(0f, GetWarningMultiplier(rayPhase));

            if (rayPhase >= 3)
            {
                yield return new WaitForSeconds(finalPhaseSecondRayDelay);
                float side = Random.value < 0.5f ? -1f : 1f;
                float offset = side * dangerZoneSize.x * 1.15f;
                SpawnDangerZone(offset, GetWarningMultiplier(rayPhase));
            }

            yield return new WaitForSeconds(dangerZoneCooldown * GetCooldownMultiplier());
        }

        attackLoopCoroutine = null;
    }

    public void StartCombat()
    {
        if (combatStarted || defeated)
        {
            return;
        }

        combatStarted = true;

        if (bossHealthBar != null)
        {
            bossHealthBar.SetActive(true);
            BossHealthBar healthBar = bossHealthBar.GetComponent<BossHealthBar>();
            if (healthBar != null)
            {
                healthBar.SetBoss(enemyHealth);
            }
        }

        StartAttackLoop();
        Debug.Log("Combate contra Hades iniciado.");
    }

    public void Petrify(float duration)
    {
        if (defeated || duration <= 0f)
        {
            return;
        }

        if (petrifyCoroutine != null)
        {
            StopCoroutine(petrifyCoroutine);
        }

        petrifyCoroutine = StartCoroutine(PetrifyRoutine(duration));
    }

    private IEnumerator PetrifyRoutine(float duration)
    {
        isPetrified = true;
        StopAttackLoop();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }

        yield return new WaitForSeconds(duration);

        isPetrified = false;
        petrifyCoroutine = null;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (combatStarted && !defeated)
        {
            StartAttackLoop();
        }
    }

    private void StartAttackLoop()
    {
        if (attackLoopCoroutine != null || defeated || isPetrified)
        {
            return;
        }

        attackLoopCoroutine = StartCoroutine(AttackLoop());
    }

    private void StopAttackLoop()
    {
        if (attackLoopCoroutine == null)
        {
            return;
        }

        StopCoroutine(attackLoopCoroutine);
        attackLoopCoroutine = null;
    }

    private void FireProjectile(float speedMultiplier = 1f)
    {
        if (player == null || isPetrified)
        {
            return;
        }

        GameObject projectile = new GameObject("Hades_Projectile");
        projectile.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, 0f);
        projectile.transform.localScale = Vector3.one * 0.55f;
        // Los proyectiles no deben pertenecer a Enemy: así el ataque cuerpo a
        // cuerpo solo detecta a Hades y el proyectil sigue dañando al jugador.
        projectile.layer = LayerMask.NameToLayer("Default");

        SpriteRenderer renderer = projectile.AddComponent<SpriteRenderer>();
        renderer.sprite = GetProjectileVisualSprite();
        renderer.color = new Color(0.8f, 0.1f, 1f, 1f);
        renderer.sortingLayerName = EffectsSortingLayer;
        renderer.sortingOrder = EffectsSortingOrder;

        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.radius = 0.25f;

        Rigidbody2D rb = projectile.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        HadesProjectile hadesProjectile = projectile.AddComponent<HadesProjectile>();
        Vector2 direction = player.position - projectile.transform.position;
        hadesProjectile.Initialize(direction, projectileSpeed * speedMultiplier, projectileDamage, projectileLifetime);
    }

    private void SpawnDangerZone(float horizontalOffset = 0f, float warningMultiplier = 1f)
    {
        if (player == null || isPetrified)
        {
            return;
        }

        GameObject zone = new GameObject("Hades_DangerZone");
        zone.transform.position = new Vector3(player.position.x + horizontalOffset, player.position.y, 0f);
        zone.transform.localScale = new Vector3(dangerZoneSize.x, dangerZoneSize.y, 1f);

        SpriteRenderer renderer = zone.AddComponent<SpriteRenderer>();
        renderer.sprite = GetDangerZoneVisualSprite();
        renderer.color = new Color(1f, 0.85f, 0.1f, 0.45f);
        renderer.sortingLayerName = EffectsSortingLayer;
        renderer.sortingOrder = EffectsSortingOrder;

        BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;

        HadesDangerZone dangerZone = zone.AddComponent<HadesDangerZone>();
        dangerZone.Initialize(dangerZoneDamage, warningTime * warningMultiplier, activeTime);
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0 || defeated)
        {
            return;
        }

        float healthPercent = (float)currentHealth / maxHealth;
        int nextPhase = healthPercent <= phaseThreeHealthPercent ? 3
            : healthPercent <= phaseTwoHealthPercent ? 2
            : 1;

        if (nextPhase == currentPhase)
        {
            return;
        }

        currentPhase = nextPhase;
        Debug.Log($"Hades entra en fase {currentPhase}. Vida restante: {healthPercent:P0}");
    }

    private float GetCooldownMultiplier()
    {
        return currentPhase >= 3 ? phaseThreeCooldownMultiplier
            : currentPhase == 2 ? phaseTwoCooldownMultiplier
            : 1f;
    }

    private float GetProjectileSpeedMultiplier(int phase)
    {
        return phase >= 3 ? phaseThreeProjectileSpeedMultiplier
            : phase == 2 ? phaseTwoProjectileSpeedMultiplier
            : 1f;
    }

    private static float GetWarningMultiplier(int phase)
    {
        return phase >= 3 ? 0.65f : phase == 2 ? 0.82f : 1f;
    }

    private void HandleDefeat()
    {
        defeated = true;
        StopAttackLoop();
        if (petrifyCoroutine != null)
        {
            StopCoroutine(petrifyCoroutine);
            petrifyCoroutine = null;
        }
        isPetrified = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

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

        if (bossArenaGate != null)
        {
            bossArenaGate.Open();
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

    private static Sprite GetProjectileVisualSprite()
    {
        if (projectileVisualSprite != null)
        {
            return projectileVisualSprite;
        }

        const int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "HadesProjectileVisual";

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.45f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        projectileVisualSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return projectileVisualSprite;
    }

    private static Sprite GetDangerZoneVisualSprite()
    {
        if (dangerZoneVisualSprite != null)
        {
            return dangerZoneVisualSprite;
        }

        Texture2D texture = Texture2D.whiteTexture;
        // La textura blanca interna puede medir varios pixeles. Usar 1 PPU hacía
        // que la sombra se viera varias veces más grande que el collider y el rayo.
        // Con el ancho como PPU, el sprite siempre ocupa exactamente 1 x 1 unidad.
        dangerZoneVisualSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            texture.width);
        return dangerZoneVisualSprite;
    }
}

[DisallowMultipleComponent]
public class VictoryMenuController : MonoBehaviour
{
    private void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (button.name == "Btn_Reiniciar")
            {
                button.onClick.AddListener(RestartLevel);
            }
            else if (button.name == "Btn_MenuPrincipal")
            {
                button.onClick.AddListener(ReturnToMenu);
            }
        }
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
