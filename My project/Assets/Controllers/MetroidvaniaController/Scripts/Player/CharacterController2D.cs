using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_WallCheck;
    [SerializeField] private float m_FastFallMultiplier = 4.5f;

    const float k_GroundedRadius = .2f;
    private bool m_Grounded;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;
    private Vector3 velocity = Vector3.zero;
    private float limitFallSpeed = 25f;

    public bool canDoubleJump = true;
    [SerializeField] private float m_DashForce = 25f;
    [SerializeField] private float m_DashCooldownDuration = 0.6f;
    private const float k_DashActiveTime = 0.1f;
    private float m_DashCooldownEndTime;
    private bool canDash = true;
    private bool isDashing = false;

    public float DashCooldownDuration => m_DashCooldownDuration;
    public float DashCooldownRemaining => Mathf.Max(0f, m_DashCooldownEndTime - Time.time);
    private bool m_IsWall = false;
    private bool isWallSliding = false;
    private bool oldWallSlidding = false;
    private float prevVelocityX = 0f;
    private bool canCheck = false;
    private bool canMove = true;

    private Animator animator;
    public ParticleSystem particleJumpUp;
    public ParticleSystem particleJumpDown;

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0;
    private bool limitVelOnWallJump = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnFallEvent;
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (OnFallEvent == null)
            OnFallEvent = new UnityEvent();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    if (!m_IsWall && !isDashing && particleJumpDown != null)
                        particleJumpDown.Play();
                    canDoubleJump = true;
                    if (m_Rigidbody2D.linearVelocity.y < 0f)
                        limitVelOnWallJump = false;
                }
            }
        }

        m_IsWall = false;

        if (!m_Grounded)
        {
            OnFallEvent.Invoke();
            Collider2D[] collidersWall = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < collidersWall.Length; i++)
            {
                if (collidersWall[i].gameObject != null)
                {
                    isDashing = false;
                    m_IsWall = true;
                }
            }
            prevVelocityX = m_Rigidbody2D.linearVelocity.x;
        }

        if (limitVelOnWallJump)
        {
            if (m_Rigidbody2D.linearVelocity.y < -0.5f)
            {
                limitVelOnWallJump = false;
                canMove = true;
            }
            jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;
            if (jumpWallDistX < -0.5f && jumpWallDistX > -1f)
            {
                canMove = true;
            }
            else if (jumpWallDistX < -1f && jumpWallDistX >= -2f)
            {
                canMove = true;
                m_Rigidbody2D.linearVelocity = new Vector2(10f * transform.localScale.x, m_Rigidbody2D.linearVelocity.y);
            }
            else if (jumpWallDistX < -2f)
            {
                limitVelOnWallJump = false;
                canMove = true;
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
            }
            else if (jumpWallDistX > 0)
            {
                limitVelOnWallJump = false;
                canMove = true;
                m_Rigidbody2D.linearVelocity = new Vector2(0, m_Rigidbody2D.linearVelocity.y);
            }
        }
    }

    public void Move(float move, bool jump, bool dash, bool fastFall)
    {
        if (canMove)
        {
            if (!m_Grounded && fastFall && m_Rigidbody2D.linearVelocity.y < 0f)
            {
                m_Rigidbody2D.linearVelocity += Vector2.up * Physics2D.gravity.y * (m_FastFallMultiplier - 1f) * Time.fixedDeltaTime;
            }

            if (dash && canDash && !isWallSliding)
            {
                StartCoroutine(DashCooldown());
            }
            if (isDashing)
            {
                m_Rigidbody2D.linearVelocity = new Vector2(transform.localScale.x * m_DashForce, 0);
            }
            else if (m_Grounded || m_AirControl)
            {
                if (m_Rigidbody2D.linearVelocity.y < -limitFallSpeed)
                    m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, -limitFallSpeed);

                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
                m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref velocity, m_MovementSmoothing);

                if (move > 0 && !m_FacingRight && !isWallSliding)
                {
                    Flip();
                }
                else if (move < 0 && m_FacingRight && !isWallSliding)
                {
                    Flip();
                }
            }

            if (m_Grounded && jump)
            {
                if (animator != null)
                {
                    animator.SetBool("IsJumping", true);
                    animator.SetBool("JumpUp", true);
                }
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                canDoubleJump = true;
                if (particleJumpDown != null) particleJumpDown.Play();
                if (particleJumpUp != null) particleJumpUp.Play();
            }
            else if (!m_Grounded && jump && canDoubleJump && !isWallSliding)
            {
                canDoubleJump = false;
                m_Rigidbody2D.linearVelocity = new Vector2(m_Rigidbody2D.linearVelocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));
                if (animator != null) animator.SetBool("IsDoubleJumping", true);
            }
            else if (m_IsWall && !m_Grounded)
            {
                if (!oldWallSlidding && m_Rigidbody2D.linearVelocity.y < 0 || isDashing)
                {
                    isWallSliding = true;
                    m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
                    Flip();
                    StartCoroutine(WaitToCheck(0.1f));
                    canDoubleJump = true;
                    if (animator != null) animator.SetBool("IsWallSliding", true);
                }
                isDashing = false;

                if (isWallSliding)
                {
                    if (move * transform.localScale.x > 0.1f)
                    {
                        StartCoroutine(WaitToEndSliding());
                    }
                    else
                    {
                        oldWallSlidding = true;
                        m_Rigidbody2D.linearVelocity = new Vector2(-transform.localScale.x * 2, -5);
                    }
                }

                if (jump && isWallSliding)
                {
                    if (animator != null)
                    {
                        animator.SetBool("IsJumping", true);
                        animator.SetBool("JumpUp", true);
                    }
                    m_Rigidbody2D.linearVelocity = new Vector2(0f, 0f);
                    m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_JumpForce * 1.2f, m_JumpForce));
                    jumpWallStartX = transform.position.x;
                    limitVelOnWallJump = true;
                    canDoubleJump = true;
                    isWallSliding = false;
                    if (animator != null) animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    canMove = false;
                }
                else if (dash && canDash)
                {
                    isWallSliding = false;
                    if (animator != null) animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    canDoubleJump = true;
                    StartCoroutine(DashCooldown());
                }
            }
            else if (isWallSliding && !m_IsWall && canCheck)
            {
                isWallSliding = false;
                if (animator != null) animator.SetBool("IsWallSliding", false);
                oldWallSlidding = false;
                m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                canDoubleJump = true;
            }
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ApplyDamage(float damage, Vector3 position)
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(Mathf.CeilToInt(Mathf.Abs(damage)));
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled && m_Rigidbody2D != null)
        {
            m_Rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
    IEnumerator DashCooldown()
    {
        if (animator != null) animator.SetBool("IsDashing", true);
        isDashing = true;
        canDash = false;
        m_DashCooldownEndTime = Time.time + m_DashCooldownDuration;
        yield return new WaitForSeconds(k_DashActiveTime);
        isDashing = false;
        yield return new WaitForSeconds(Mathf.Max(0f, m_DashCooldownDuration - k_DashActiveTime));
        canDash = true;
    }
    IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        canDoubleJump = true;
        isWallSliding = false;
        if (animator != null) animator.SetBool("IsWallSliding", false);
        oldWallSlidding = false;
        m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
    }
}