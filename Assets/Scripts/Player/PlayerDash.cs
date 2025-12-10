using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    InputSystemActions inputActions;
    PlayerStates ps;
    Rigidbody rb;

    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float cooldownTimer = 0f;

    Vector3 dashDirection;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        ps = GetComponent<PlayerStates>();
        rb = GetComponent<Rigidbody>();

        inputActions = new InputSystemActions();

        inputActions.Player.Interaction.started += OnDash;
    }

    void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("Dash");

        if (ps.canDash)
        {
            rb.AddForce(ps.GetFacingToDirection() * dashForce, ForceMode.VelocityChange);
        }
    }

    
}
