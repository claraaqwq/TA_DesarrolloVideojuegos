using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MinotaurEnemy : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        ChargeTelegraph,
        Charging,
        ChargeRecovery,
        SlamTelegraph,
        SlamRecovery
    }

    [Header("Movimiento (lento)")]
    [SerializeField] private float patrolSpeed = 1.2f;
    [SerializeField] private float chaseSpeed = 1.8f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float aggroRange = 6f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Embestida (Charge)")]
    [SerializeField] private float chargeRange = 5f;
    [SerializeField] private float chargeMinRange = 1.6f;
    [SerializeField] private float chargeHeightTolerance = 1f;
    [SerializeField] private float chargeTelegraphTime = 0.6f;
    [SerializeField] private float chargeSpeed = 9f;
    [SerializeField] private float chargeMaxDistance = 8f;
    [SerializeField] private float chargeHitRadius = 0.6f;
    [SerializeField] private float chargeRecoveryTime = 1.2f;
    [SerializeField] private float chargeCooldown = 2.5f;
    [SerializeField] private int chargeDamage = 2;

    [Header("Pisotón (Slam)")]
    [SerializeField] private float slamRange = 1.6f;
    [SerializeField] private float slamTelegraphTime = 0.5f;
    [SerializeField] private float slamRadius = 1.8f;
    [SerializeField] private int slamDamage = 1;
    [SerializeField] private float slamRecoveryTime = 0.4f;
    [SerializeField] private float slamCooldown = 2f;

    [Header("Daño de contacto")]
    [SerializeField] private int contactDamage = 1;

    [Header("UI")]
    [SerializeField] private GameObject bossHealthBar;

    [Header("Animación (opcional)")]
    [SerializeField] private Animator animator;

    private EnemyHealth enemyHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Transform player;
    private Vector3 startPosition;
    private bool facingRight = true;
    private State state = State.Patrol;
    private float chargeCooldownRemaining;
    private float slamCooldownRemaining;
    private bool isDead;
    private bool isKnockedBack;
    private bool healthBarRevealed;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.Died += HandleDeath;
        enemyHealth.KnockedBack += HandleKnockback;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= HandleDeath;
            enemyHealth.KnockedBack -= HandleKnockback;
        }
    }

    private void Start()
    {
        startPosition = transform.position;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        StartCoroutine(BehaviorLoop());
    }

    private void Update()
    {
        if (chargeCooldownRemaining > 0f)
        {
            chargeCooldownRemaining -= Time.deltaTime;
        }

        if (slamCooldownRemaining > 0f)
        {
            slamCooldownRemaining -= Time.deltaTime;
        }
    }

    private IEnumerator BehaviorLoop()
    {
        while (!isDead)
        {
            if (isKnockedBack || player == null)
            {
                yield return null;
                continue;
            }

            if (!healthBarRevealed && (PlayerInAggroRange() || PlayerInChargeRange() || PlayerInSlamRange()))
            {
                RevealHealthBar();
            }

            if (chargeCooldownRemaining <= 0f && PlayerInChargeRange())
            {
                yield return StartCoroutine(ChargeAttack());
            }
            else if (slamCooldownRemaining <= 0f && PlayerInSlamRange())
            {
                yield return StartCoroutine(SlamAttack());
            }
            else if (PlayerInAggroRange())
            {
                state = State.Chase;
                ChaseStep();
                yield return null;
            }
            else
            {
                state = State.Patrol;
                PatrolStep();
                yield return null;
            }
        }
    }

    private void PatrolStep()
    {
        int direction = facingRight ? 1 : -1;
        float distanceFromStart = transform.position.x - startPosition.x;

        transform.position += Vector3.right * direction * patrolSpeed * Time.deltaTime;
        SetAnimatorSpeed(patrolSpeed);

        if (direction == 1 && distanceFromStart >= patrolDistance)
        {
            Flip();
        }
        else if (direction == -1 && distanceFromStart <= -patrolDistance)
        {
            Flip();
        }
    }

    private void ChaseStep()
    {
        int direction = player.position.x >= transform.position.x ? 1 : -1;
        FaceDirection(direction);

        if (Mathf.Abs(player.position.x - transform.position.x) > chargeMinRange)
        {
            transform.position += Vector3.right * direction * chaseSpeed * Time.deltaTime;
            SetAnimatorSpeed(chaseSpeed);
        }
        else
        {
            SetAnimatorSpeed(0f);
        }
    }

    private void SetAnimatorSpeed(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    private IEnumerator ChargeAttack()
    {
        state = State.ChargeTelegraph;

        int direction = player.position.x >= transform.position.x ? 1 : -1;
        FaceDirection(direction);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.5f, 0.25f, 1f);
        }

        if (animator != null)
        {
            animator.SetTrigger("ChargeTelegraph");
        }

        yield return new WaitForSeconds(chargeTelegraphTime);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        state = State.Charging;
        bool hasHitPlayer = false;
        float traveled = 0f;

        if (animator != null)
        {
            animator.SetBool("Charging", true);
        }

        while (traveled < chargeMaxDistance)
        {
            if (Physics2D.Raycast(transform.position, Vector2.right * direction, 0.4f, wallLayer))
            {
                break;
            }

            float step = chargeSpeed * Time.deltaTime;
            transform.position += Vector3.right * direction * step;
            traveled += step;

            if (!hasHitPlayer && player != null && Vector2.Distance(transform.position, player.position) <= chargeHitRadius)
            {
                PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();
                if (playerHealth != null)
                {
                    hasHitPlayer = true;
                    playerHealth.TakeDamage(chargeDamage, new Vector2(direction, 0f));
                }
            }

            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("Charging", false);
        }

        state = State.ChargeRecovery;
        yield return new WaitForSeconds(chargeRecoveryTime);

        chargeCooldownRemaining = chargeCooldown;
        state = State.Chase;
    }

    private IEnumerator SlamAttack()
    {
        state = State.SlamTelegraph;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.85f, 0.2f, 1f);
        }

        if (animator != null)
        {
            animator.SetTrigger("Slam");
        }

        yield return new WaitForSeconds(slamTelegraphTime);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        state = State.SlamRecovery;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);
        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector2 knockbackDirection = playerHealth.transform.position - transform.position;
                playerHealth.TakeDamage(slamDamage, knockbackDirection);
            }
        }

        yield return new WaitForSeconds(slamRecoveryTime);

        slamCooldownRemaining = slamCooldown;
        state = State.Chase;
    }

    private bool PlayerInAggroRange()
    {
        return Vector2.Distance(transform.position, player.position) <= aggroRange;
    }

    private bool PlayerInChargeRange()
    {
        float dx = Mathf.Abs(player.position.x - transform.position.x);
        float dy = Mathf.Abs(player.position.y - transform.position.y);
        return dx <= chargeRange && dx >= chargeMinRange && dy <= chargeHeightTolerance;
    }

    private bool PlayerInSlamRange()
    {
        return Vector2.Distance(transform.position, player.position) <= slamRange;
    }

    private void FaceDirection(int direction)
    {
        if (direction > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction < 0 && facingRight)
        {
            Flip();
        }
    }

    private void RevealHealthBar()
    {
        healthBarRevealed = true;

        if (bossHealthBar != null)
        {
            bossHealthBar.SetActive(true);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        StopAllCoroutines();

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    private void HandleKnockback(Vector2 direction, float distance, float duration)
    {
        if (isDead)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        StartCoroutine(KnockbackRoutine(direction, distance, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float distance, float duration)
    {
        isKnockedBack = true;

        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(direction * distance);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        transform.position = end;
        isKnockedBack = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == State.Charging || state == State.ChargeTelegraph)
        {
            return;
        }

        PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            Vector2 knockbackDirection = playerHealth.transform.position - transform.position;
            playerHealth.TakeDamage(contactDamage, knockbackDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = new Color(1f, 0.5f, 0.25f, 1f);
        Gizmos.DrawWireSphere(transform.position, chargeRange);

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 1f);
        Gizmos.DrawWireSphere(transform.position, slamRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRadius);
    }
}
