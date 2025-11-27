using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public bool isMoving;
    public bool isJumping;
    public bool isSprinting;
    public bool isInteracting;
    public bool isActiveCharacter;

    public bool canMove = true;
    public bool canSprint = true;
    public bool canJump = true;
    public bool canInteract = true;
    public bool canCharacterSwitch = true;

    public enum Facing { Left, Right, Face, Back }
    public Facing facing = Facing.Face;

    public void LockMovement()
    {
        canMove = false;
        canSprint = false;
        canJump = false;
        canInteract = false;
    }

    public void LockInteraction()
    {
        canInteract = false;
    }

    public void DeLockMovement()
    {
        canMove = true;
        canSprint = true;
        canJump = true;
        canInteract = true;
    }

    public void DeLockInteraction()
    {
        canInteract = false;
    }
}
