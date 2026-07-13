using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private int damage = 4;
    [SerializeField] private float attackRange = 1.4f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Cooldown")]
    public float attackCooldown = 0.25f;
    private bool canAttack = true;

    [Header("Animación")]
    public Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        if (animator != null)
        {
            animator.SetBool("Attack", true);
        }

        // Espera pequeña para que coincida con el golpe de la animación
        yield return new WaitForSeconds(0.1f);

        DoAttackDamage();

        yield return new WaitForSeconds(attackCooldown);

        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }

        canAttack = true;
    }

    void DoAttackDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("PlayerAttack no tiene AttackPoint asignado.");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        Debug.Log($"PlayerAttack: {hitEnemies.Length} colliders detectados en el rango de ataque.");

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                Vector2 knockbackDirection = enemyHealth.transform.position - attackPoint.position;
                Debug.Log($"PlayerAttack: golpeando a {enemyHealth.name} por {damage} de daño.");
                enemyHealth.TakeDamage(damage, knockbackDirection);
            }
            else
            {
                Debug.LogWarning($"PlayerAttack: {enemy.name} esta en la capa de enemigos pero no tiene EnemyHealth (ni en si mismo ni en sus padres).");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
