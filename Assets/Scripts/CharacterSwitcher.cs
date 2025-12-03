using UnityEngine;
using System.Collections.Generic;
public class CharacterSwitcher : MonoBehaviour
{
    // Singleton pour accès facile
    public static CharacterSwitcher Instance { get; private set; }

    public List<PlayerStates> charactersStates; // Référence à Perséphone et Hadès
    private int currentIndex = 0;
    private bool canCharacterSwitch = true;

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
    }

    void Start()
    {
        // Assurer que seul le premier est actif au début
        ActivateCharacter(currentIndex);
        
        // S'abonner à l'événement de changement d'état du GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
        }
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
        // [CONTRÔLE D'ÉTAT] : Bloquer le switch si l'état est dans un autre mode que "Exploration"
        if (!canCharacterSwitch) 
        {
            Debug.Log($"Switch refusé : Le jeu est en mode {GameManager.Instance.GetCurrentState()}.");
            return;
        }
        // 1. Désactiver le personnage actuel
        PlayerStates oldState = charactersStates[currentIndex];
        oldState.LockMovement();
        oldState.isActiveCharacter = false;

        // 2. Changer l'index (boucle)
        currentIndex = (currentIndex + 1) % charactersStates.Count;

        // 3. Activer le nouveau personnage
        ActivateCharacter(currentIndex);
        
        // TODO: Logique de camera switch(cinemachine ou autre)
        Debug.Log($"Switch vers : {charactersStates[currentIndex].gameObject.name}");
    }
    
    private void ActivateCharacter(int index)
    {
        PlayerStates newState = charactersStates[index];
        newState.isActiveCharacter = true; 
        newState.DeLockMovement(); 
        newState.canCharacterSwitch = canCharacterSwitch; // Permettre ou non le switch
    }
    
}