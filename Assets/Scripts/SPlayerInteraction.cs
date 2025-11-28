using UnityEngine;
using UnityEngine.InputSystem;

public class SPlayerInteraction : MonoBehaviour
{
    PlayerStates ps;

    InputSystemActions inputActions;

    [SerializeField] SCheckInteractionObject interactionCollider;

    [SerializeField] float interactionDistance;
    [SerializeField] LayerMask layerTarget;

    [SerializeField] Vector3 direction;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        ps = GetComponent<PlayerStates>();

        inputActions = new InputSystemActions();

        inputActions.Player.Interaction.started += OnInteraction;
    }

    private void Update()
    {
        if (ps.facing == PlayerStates.Facing.Left) direction = Vector3.left;
        if (ps.facing == PlayerStates.Facing.Right) direction = Vector3.right;
        if (ps.facing == PlayerStates.Facing.Face) direction = Vector3.forward;
        if (ps.facing == PlayerStates.Facing.Back) direction = Vector3.back;
    }

    void OnInteraction(InputAction.CallbackContext context)
    {
        if (ps.canInteract)
        {
            if (
                interactionCollider.colliderObject != null
                && interactionCollider.colliderObject.TryGetComponent<IInteractable>(out var interactable)
            )
            {
                interactable.OnInteractStart(this);
            }
        }
    }
}
