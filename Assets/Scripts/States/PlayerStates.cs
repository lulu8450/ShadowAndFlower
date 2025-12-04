using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    [Header("États du Joueur")]
    public bool isMoving; // Indique si le joueur est en train de se déplacer
    public bool isJumping; // Indique si le joueur est en train de sauter
    public bool isSprinting; // Indique si le joueur est en train de sprinter
    public bool isInteracting; // Indique si le joueur intérragit actuellement avec un objet
    public bool isTriggerInteracting; // Indique si le joueur intérragit actuellement avec un Trigger
    public bool isActiveCharacter; // Indique si le joueur contrôle ce GameObject

    [Header("Permissions du Joueur")]
    public bool canMove; // Indique si le joueur peut se déplacer
    public bool canSprint; // Indique si le joueur peut sprinter
    public bool canJump; // Indique si le joueur peut sauter
    public bool canInteract; // Indique si le joueur peut interagir
    public bool canTriggerInteract; // Indique si le joueur peut interagir avec un trigger
    public bool canCharacterSwitch; // Indique si le joueur peut changer de personnage

    public enum Facing { Left, Right, Face, Back } // Enum pour les directions
    public Facing facing = Facing.Face; // Direction actuelle du personnage

    public void LockMovement() // Fonction de verrouillage globale
    {
        canMove = false;
        canSprint = false;
        canJump = false;
        canInteract = false;
    }

    public void DeLockMovement() // Fonction de déverrouillage globale
    {
        canMove = true;
        canSprint = true;
        canJump = true;
        canInteract = true;
    }

    public void LockInteraction() // Fonction de verrouillage de l'interaction
    {
        canInteract = false;
    }

    public void DeLockInteraction() // Fonction de déverrouillage de l'interaction
    {
        canInteract = true;
    }

    public void LockTriggerInteraction() // Fonction de verrouillage de l'interaction
    {
        canTriggerInteract = false;
    }

    public void DeLockTriggerInteraction() // Fonction de déverrouillage de l'interaction
    {
        canTriggerInteract = true;
    }
}
