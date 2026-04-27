using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool dash = false;

    void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        // Comprobamos si hay animator para evitar errores
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
    }

    public void OnFall()
    {
        if (animator != null) animator.SetBool("IsJumping", true);
    }

    public void OnLanding()
    {
        if (animator != null) animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
        jump = false;
        dash = false;
    }
}