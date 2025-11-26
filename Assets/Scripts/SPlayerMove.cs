using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SPlayerMove : MonoBehaviour
{
    InputSystemActions inputActions;
    Rigidbody rb;

    PlayerStates ps;

    [SerializeField] Transform sprite;

    [Header("Move Settings")]
    [SerializeField] float speed;

    [Header("Sprint Settings")]
    [SerializeField] float sprintMultiplier;

    Vector2 moveInput;

    //float currentRotateSpeed;
    //float targetRotateSpeed = 1f;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        ps = GetComponent<PlayerStates>();

        inputActions = new InputSystemActions();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }

    private void FixedUpdate()
    {
        if (ps.canMove)
        {
            float currentSpeed = ps.isSprinting ? speed * sprintMultiplier : speed;
            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            Vector3 velocity = direction * currentSpeed;

            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

            if (direction.magnitude > 0.1f)
            {
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    if (direction.x > 0) ps.facing = PlayerStates.Facing.Right;
                    else ps.facing = PlayerStates.Facing.Left;
                }
                else
                {
                    if (direction.z > 0) ps.facing = PlayerStates.Facing.Face;
                    else ps.facing = PlayerStates.Facing.Back;
                }
            }

            //float targetRotate = 0f;
            //if (ps.facing == PlayerStates.Facing.Left) targetRotate = 180f;

            //Quaternion targetRotation = Quaternion.Euler(0f, targetRotate, 0f);

            //sprite.rotation = Quaternion.Lerp(
            //    transform.rotation,
            //    targetRotation,
            //    currentRotateSpeed
            //);

        }


        //if (currentRotateSpeed <= targetRotateSpeed) currentRotateSpeed += 0.1f;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero) ps.isMoving = true;
        else ps.isMoving = false;
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        if (ps.canSprint)
        {
            if (context.performed) ps.isSprinting = true;
            else if (context.canceled) ps.isSprinting = false;
        }
    }
}
