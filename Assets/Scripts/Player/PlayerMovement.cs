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
    private Vector2 lastMoveDirection = Vector2.down;

    public static int ActiveMenuCount { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (SoundManager.Instance == null) return;

        bool isMoving = ActiveMenuCount == 0 && (horizontalInput != 0f || verticalInput != 0f);

        if (isMoving)
            SoundManager.Instance.StartFootsteps();
        else
            SoundManager.Instance.StopFootsteps();
    }

    private void FixedUpdate()
    {
        if (ActiveMenuCount > 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>(); // read BEFORE canceled check

        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastMoveDirection.x); // capital X
            animator.SetFloat("LastInputY", lastMoveDirection.y); // capital Y
        }
        else
        {
            lastMoveDirection = inputVector;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", horizontalInput, 0f, Time.deltaTime);
            animator.SetFloat("InputY", verticalInput, 0f, Time.deltaTime);
        }
    }
}
