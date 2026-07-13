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
    [SerializeField] private float blinkStartupDelay = 0.2f;
    [SerializeField] private float blinkHangTime = 0.5f;
    [SerializeField] private LayerMask blinkBlockLayer;
    [SerializeField] private Sprite[] blinkEffectFrames;
    [SerializeField] private float blinkEffectFrameRate = 30f;

    private Rigidbody2D rb;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool dash = false;
    private bool fastFall = false;

    private bool canBlink = true;
    private int facingDirection = 1;
    private float blinkCooldownEndTime;

    public float BlinkCooldownDuration => blinkCooldown;
    public float BlinkCooldownRemaining => Mathf.Max(0f, blinkCooldownEndTime - Time.time);

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

        fastFall = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
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
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash, fastFall);

        jump = false;
        dash = false;
    }

    private IEnumerator Blink()
    {
        canBlink = false;
        blinkCooldownEndTime = Time.time + blinkCooldown;

        // La direccion se fija al presionar E, no cuando termina de canalizar
        Vector2 direction = Vector2.right * facingDirection;

        // Fase 1: canalizar - el personaje queda quieto en el aire o en tierra
        float originalGravity = rb.gravityScale;
        controller.SetMovementEnabled(false);
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(blinkStartupDelay);

        // Fase 2: teletransporte
        Vector2 origin = rb.position;

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

        SpriteFlipbookEffect.Play(blinkEffectFrames, origin, blinkEffectFrameRate);

        rb.position = targetPosition;

        SpriteFlipbookEffect.Play(blinkEffectFrames, targetPosition, blinkEffectFrameRate);

        // Fase 3: recuperar control
        controller.SetMovementEnabled(true);

        yield return new WaitForFixedUpdate();

        // Fase 4: si quedaste en el aire, un breve respiro sin gravedad
        // (el jugador puede cortarlo antes con dash o caida rapida)
        if (!controller.IsGrounded)
        {
            float hangElapsed = 0f;

            while (hangElapsed < blinkHangTime)
            {
                if (fastFall || controller.IsDashing)
                {
                    break;
                }

                hangElapsed += Time.deltaTime;
                yield return null;
            }
        }

        rb.gravityScale = originalGravity;

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