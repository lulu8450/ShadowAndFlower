using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public bool isMoving;
    public bool isJumping;
    public bool isSprinting;

    public bool canMove = true;
    public bool canSprint = true;
    public bool canJump = true;

    public enum Facing { Left, Right, Face, Back }
    public Facing facing = Facing.Face;
}
