using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque")]
    public float damage = 4f;
    public float attackRange = 0.9f;
    public Transform attackPoint;
    public LayerMask enemyLayer;

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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}