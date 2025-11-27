using UnityEngine;
using UnityEngine.InputSystem;

public class SPlayerInteraction : MonoBehaviour
{
    PlayerStates ps;

    InputSystemActions inputActions;

    [SerializeField] float interactionDistance;
    [SerializeField] LayerMask layerTarget;

    [SerializeField] Vector3 direction;

    private void Awake()
    {
        ps = GetComponent<PlayerStates>();

        inputActions = new InputSystemActions();

        inputActions.Player.Interaction.started += OnInteraction;
    }

    void OnInteraction(InputAction.CallbackContext context)
    {
        if (ps.canInteract)
        {
            if (ps.facing == PlayerStates.Facing.Left) direction = Vector3.left;
            if (ps.facing == PlayerStates.Facing.Right) direction = Vector3.right;
            if (ps.facing == PlayerStates.Facing.Face) direction = Vector3.back;
            if (ps.facing == PlayerStates.Facing.Back) direction = Vector3.forward;
            
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, interactionDistance, layerTarget))
            {
                if (hit.collider != null && hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.OnInteractStart(this);
                    ps.canInteract = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + (direction * interactionDistance));
    }
}
