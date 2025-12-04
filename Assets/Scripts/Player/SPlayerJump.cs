using UnityEngine;
using UnityEngine.InputSystem;

public class SPlayerJump : MonoBehaviour
{
    InputSystemActions inputActions;
    Rigidbody rb;
    PlayerStates ps;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundDistance;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<PlayerStates>();

        inputActions = new InputSystemActions();

        inputActions.Player.Jump.performed += OnJump;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (IsGrounded() && ps.canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
    }
}
