using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] Rigidbody2D rb;
    [Header("Input Settings")]
    [SerializeField] float moveSpeed;
    private float horizontalInput;
    private float verticalInput;
    private Animator animator;

    /// <summary>
    /// Number of UI menus currently open. Movement is blocked when > 0.
    /// </summary>
    public static int ActiveMenuCount { get; set; }
    private void FixedUpdate()
    {
        if (ActiveMenuCount > 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);
    }
    
    
    private void Awake()
    {
    animator = GetComponent<Animator>();
    } 

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking",true);

        if(context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputx", horizontalInput);
            animator.SetFloat("LastInputy", verticalInput);
        }
        Vector2 inputVector = context.ReadValue<Vector2>();
        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;
        animator.SetFloat("InputX", horizontalInput);
        animator.SetFloat("InputY", verticalInput);
    }
}