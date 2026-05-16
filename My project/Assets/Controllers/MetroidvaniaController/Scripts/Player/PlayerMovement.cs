using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    public CharacterController2D controller;
    public Animator animator;

    [Header("Movimiento")]
    public float runSpeed = 40f;

    [Header("Blink")]
    [SerializeField] private float blinkDistance = 3f;
    [SerializeField] private float blinkCooldown = 0.5f;
    [SerializeField] private LayerMask blinkBlockLayer;

    private Rigidbody2D rb;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool dash = false;

    private bool canBlink = true;
    private int facingDirection = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float rawHorizontal = Input.GetAxisRaw("Horizontal");

        horizontalMove = rawHorizontal * runSpeed;

        if (rawHorizontal > 0)
        {
            facingDirection = 1;
        }
        else if (rawHorizontal < 0)
        {
            facingDirection = -1;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            dash = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && canBlink)
        {
            StartCoroutine(Blink());
        }
    }

    public void OnFall()
    {
        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
        }
    }

    public void OnLanding()
    {
        if (animator != null)
        {
            animator.SetBool("IsJumping", false);
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);

        jump = false;
        dash = false;
    }

    private IEnumerator Blink()
    {
        canBlink = false;

        Vector2 origin = rb.position;
        Vector2 direction = Vector2.right * facingDirection;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            blinkDistance,
            blinkBlockLayer
        );

        Vector2 targetPosition;

        if (hit.collider != null)
        {
            float ajuste = 0.3f;
            targetPosition = hit.point - direction * ajuste;
        }
        else
        {
            targetPosition = origin + direction * blinkDistance;
        }

        rb.position = targetPosition;

        /*if (animator != null)
        {
            animator.SetTrigger("Blink");
        }*/

        yield return new WaitForSeconds(blinkCooldown);

        canBlink = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 direction = Vector3.right * facingDirection;
        Gizmos.DrawLine(transform.position, transform.position + direction * blinkDistance);
    }
}