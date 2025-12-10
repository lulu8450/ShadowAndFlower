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
    public bool isFollowing; // Indique si le personnage suit le personnage controllé
    public bool isCloneCreated; // Indique si le personnage (Hades) a clone actif
    public bool isDashing; // Indique si le personnage est en train de dasher

    [Header("Permissions du Joueur")]
    public bool canMove; // Indique si le joueur peut se déplacer
    public bool canSprint; // Indique si le joueur peut sprinter
    public bool canJump; // Indique si le joueur peut sauter
    public bool canInteract; // Indique si le joueur peut interagir
    public bool canTriggerInteract; // Indique si le joueur peut interagir avec un trigger
    public bool canCharacterSwitch; // Indique si le joueur peut changer de personnage
    public bool canFollow = true; // Indique si le personnage peut suivre le joueur controllé
    public bool canMakeClone; // Indique si le personnage (Hades) peut crée un clone
    public bool canDash; // Indique si le personnage peut dash

    public enum Facing { Left, Right, Face, Back } // Enum pour les directions
    public Facing facing = Facing.Face; // Direction actuelle du personnage

    public enum Perso { Persephone, Ades, Shadow } // Enum pour les personnages
    public Perso persoType; // Personnage actuelle

    public enum Following { Follow, Stay } // Enum pour le following des persos
    public Following following; // Etat du follow actuel

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

    // Fonction de verrouillage de l'interaction
    public void LockInteraction() { canInteract = false; }

    // Fonction de déverrouillage de l'interaction
    public void DeLockInteraction() { canInteract = true; }

    // Fonction de verrouillage de l'interaction
    public void LockTriggerInteraction() { canTriggerInteract = false; }

    // Fonction de déverrouillage de l'interaction
    public void DeLockTriggerInteraction() { canTriggerInteract = true; }

    public Vector3 GetFacingToDirection()
    {
        Vector3 direction = Vector3.zero;

        if (gameObject.GetComponent<PlayerStates>().facing == Facing.Left) direction = Vector3.left;
        if (gameObject.GetComponent<PlayerStates>().facing == Facing.Right) direction = Vector3.right;
        if (gameObject.GetComponent<PlayerStates>().facing == Facing.Face) direction = Vector3.forward;
        if (gameObject.GetComponent<PlayerStates>().facing == Facing.Back) direction = Vector3.back;

        return direction;
    }
}
