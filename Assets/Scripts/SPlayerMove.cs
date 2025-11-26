using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SPlayerMove : MonoBehaviour
{
    InputSystemActions inputActions;
    Rigidbody2D rb;

    [SerializeField] float speed;

    bool canMove = true;

    public enum Facing { Left, Right, Up, Down }
    public Facing facing = Facing.Right;

    Vector3 moveInput;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        //ps = GetComponent<PlayerState>();
        rb = GetComponent<Rigidbody2D>();

        inputActions = new InputSystemActions();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void Update()
    {
        if (canMove)
        {

        }
    }

    void OnMove(InputAction.CallbackContext context)
    {

    }
}
