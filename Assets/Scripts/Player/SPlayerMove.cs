using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Rotation Sprite Settings")]
    [SerializeField] float rotateDuration = 0.2f;
    float rotateTimer = 0f;
    bool rotating = false;
    Quaternion rotateStart;
    Quaternion rotateEnd;

    Vector2 moveInput;

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

    private void Update()
    {
        if (rotating)
        {
            rotateTimer += Time.deltaTime;
            float t = rotateTimer / rotateDuration;

            sprite.rotation = Quaternion.Slerp(rotateStart, rotateEnd, t);

            if (t >= 1f)
            {
                rotating = false;
                sprite.rotation = rotateEnd;
            }
        }
    }

    private void FixedUpdate()
    {
        //TODO : mettre des triggers
        if (ps.canMove)
        {
            Camera camera = Camera.main;
            var forward = camera.transform.forward;
            var right = camera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            float currentSpeed = ps.isSprinting ? speed * sprintMultiplier : speed;
            Vector3 direction = new Vector3((forward * moveInput.x).x, 0f, -moveInput.y).normalized;
            Vector3 velocity = direction * currentSpeed;

            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

            UpdateFacing(direction);

        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
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

    void RotateSprite(Vector3 direction)
    {
        float targetY = direction.x > 0 ? 0f : 180f;

        rotating = true;
        rotateTimer = 0f;
        rotateStart = sprite.rotation;
        rotateEnd = Quaternion.Euler(0f, targetY, 0f);
    }

    public void UpdateFacing(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            if (Mathf.Abs(direction.x) > 0.05f)
            {
                if (direction.x > 0) ps.facing = PlayerStates.Facing.Right;
                else ps.facing = PlayerStates.Facing.Left;

                RotateSprite(direction);
            }

            if (Mathf.Abs(direction.z) > Mathf.Abs(direction.x))
            {
                if (direction.z > 0) ps.facing = PlayerStates.Facing.Face;
                else ps.facing = PlayerStates.Facing.Back;
            }
        }
    }

    public float GetSpeed() => speed;
    public float GetSprintMultiplicator() => sprintMultiplier;
}
