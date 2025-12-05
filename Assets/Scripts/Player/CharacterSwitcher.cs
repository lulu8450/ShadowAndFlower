using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    InputSystemActions inputActions;

    // Singleton pour accès facile
    public static CharacterSwitcher Instance { get; private set; }

    public List<PlayerStates> charactersStates; // Référence à Perséphone et Hadès
    public int currentIndex = 0;
    public bool canCharacterSwitch = true;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        inputActions = new InputSystemActions();

        inputActions.Player.SwitchCharacter.started += OnSwitchAction;
    }

    void Start()
    {
        // Assurer que seul le premier est actif au début
        if (charactersStates != null && charactersStates.Count > 0)
        {
            ActivateCharacter(currentIndex);
        }
        
        // S'abonner à l'événement de changement d'état du GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
        }
    }

    private void OnSwitchAction(InputAction.CallbackContext ctx)
    {
        // Only react on performed phase (already ensured by subscription)
        SwitchCharacter();
    }
    
    private void OnGameStateChanged(GameState newState)
    {
        // Le switch est seulement autorisé en Exploration
        canCharacterSwitch = newState == GameState.Exploration;
        
        // Mettre à jour la permission de switch dans le PlayerStates actif
        if (charactersStates.Count > currentIndex && charactersStates[currentIndex] != null)
        {
             charactersStates[currentIndex].canCharacterSwitch = canCharacterSwitch; 
        }
    }

    public void SwitchCharacter()
    {
        //Debug.Log("Je marche");
        // [CONTRÔLE D'ÉTAT] : Bloquer le switch si l'état est dans un autre mode que "Exploration"
        if (!canCharacterSwitch) 
        {
            Debug.Log($"Switch refusé : Le jeu est en mode {GameManager.Instance.GetCurrentState()}.");
            return;
        }
        if (charactersStates == null || charactersStates.Count == 0)
        {
            Debug.LogWarning("Aucun personnage disponible pour le switch.");
            return;
        }

        // 1. Désactiver le personnage actuel
        PlayerStates oldState = charactersStates[currentIndex];
        if (oldState != null)
        {
            oldState.LockMovement();
            oldState.isActiveCharacter = false;
            oldState.canFollow = !oldState.isActiveCharacter;
        }

        // 2. Changer l'index (boucle)
        currentIndex = (currentIndex + 1) % charactersStates.Count;

        // 3. Activer le nouveau personnage
        ActivateCharacter(currentIndex);
        
        // TODO: Logique de camera switch(cinemachine ou autre)
        Debug.Log($"Switch vers : {charactersStates[currentIndex].gameObject.name}");
    }
    
    private void ActivateCharacter(int index)
    {
        // if (charactersStates == null || index < 0 || index >= charactersStates.Count) return;

        PlayerStates newState = charactersStates[index];
        if (newState == null) return;

        newState.isActiveCharacter = true; 
        newState.DeLockMovement(); 
        newState.canCharacterSwitch = canCharacterSwitch; // Permettre ou non le switch
        newState.canFollow = !newState.isActiveCharacter;
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.RemoveListener(OnGameStateChanged);
        }
    }

}