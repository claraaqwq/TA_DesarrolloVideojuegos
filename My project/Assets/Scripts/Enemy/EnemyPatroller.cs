using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;
    public int damage = 1;

    private Vector3 startPosition;
    private int direction = 1;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        float distanceFromStart = transform.position.x - startPosition.x;

        if (direction == 1 && distanceFromStart >= moveDistance)
        {
            direction = -1;
            FlipSprite();
        }
        else if (direction == -1 && distanceFromStart <= -moveDistance)
        {
            direction = 1;
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction == -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}