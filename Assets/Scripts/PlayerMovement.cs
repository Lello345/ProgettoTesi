using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DialogueView.OnDialogueOpened += DisableMovement;
        DialogueView.OnDialogueClosed += EnableMovement;
    }

    private void EnableMovement()
    {
        InputActionMap playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        playerActionMap?.Enable();
    }

    private void DisableMovement()
    {
        InputActionMap playerActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        playerActionMap?.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * movementSpeed;
    }


    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

    }
}
