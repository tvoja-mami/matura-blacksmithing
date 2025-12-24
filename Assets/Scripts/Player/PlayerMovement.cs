using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMOvement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float moveSpeed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
