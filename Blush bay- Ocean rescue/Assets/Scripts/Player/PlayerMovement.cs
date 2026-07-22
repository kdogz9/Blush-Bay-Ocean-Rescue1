using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float MovementSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 MovementInput;
    private Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = MovementInput * MovementSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", MovementInput.x);
            animator.SetFloat("LastInputY", MovementInput.y);
        }
        MovementInput = context.ReadValue<Vector2>();
        
        animator.SetFloat("InputX", MovementInput.x);
        
        animator.SetFloat("InputY", MovementInput.y);
    }
}
