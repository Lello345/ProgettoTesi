using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * movementSpeed;

        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }


    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;

        if (context.canceled || moveInput == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
            
            // 2. Aggiorna i parametri solo quando il giocatore si sta effettivamente muovendo
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);

            // 3. Salva l'ultimo input valido per la posizione d'arresto (Idle)
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
            
        }

    }
}
