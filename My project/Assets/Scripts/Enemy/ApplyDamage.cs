using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 10f;

    public void ApplyDamage(float damage)
    {
        health -= damage;

        Debug.Log("Enemigo recibió daño: " + damage);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}