using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyDropOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private Vector3 spawnOffset;

    private EnemyHealth enemyHealth;
    private bool hasDropped;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.Died += Drop;
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= Drop;
        }
    }

    private void Drop()
    {
        if (hasDropped || dropPrefab == null)
        {
            return;
        }

        hasDropped = true;
        Instantiate(dropPrefab, transform.position + spawnOffset, Quaternion.identity);
    }
}
