using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Ajouté pour le TryGetComponent, si besoin.
public class CharacterSwitcher : MonoBehaviour
{
    // Singleton pour accès facile
    public static CharacterSwitcher Instance { get; private set; }

    public List<CharacterController> characters; // Référence à Perséphone et Hadès
    private int currentIndex = 0;

    // Nouveau champ : contrôle si le switch est autorisé
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
    // Fonction appelée par l'événement du GameManager
    private void OnGameStateChanged(GameState newState)
    {
        // Le switch est seulement autorisé en Exploration
        canCharacterSwitch = newState == GameState.Exploration;
        
        // Optionnel : tu peux utiliser PlayerStates.cs (que tu as uploadé) ici pour plus de détails
        characters[currentIndex].GetComponent<PlayerStates>().canCharacterSwitch = canCharacterSwitch;
    }
    public void SwitchCharacter()
    {
        // CONTRÔLE D'ÉTAT AJOUTÉ : Vérifier si on est en état d'exploration
        if (!canCharacterSwitch) 
        {
            Debug.Log($"Switch refusé : Le jeu est en mode {GameManager.Instance.GetCurrentState()}.");
            return;
        }
        // 1. Désactiver le personnage actuel
        // Utiliser les fonctions Lock/DeLock de PlayerStates.cs pour plus de contrôle
        characters[currentIndex].GetComponent<PlayerStates>().LockMovement(); // Bloquer l'ancien
        characters[currentIndex].GetComponent<PlayerStates>().isActiveCharacter = false;

        // 2. Changer l'index (boucle)
        currentIndex = (currentIndex + 1) % characters.Count;

        // 3. Activer le nouveau personnage
        ActivateCharacter(currentIndex);
        
        // Logique de caméra : le Dév 2 devra déplacer la caméra vers le nouveau personnage.
        Debug.Log($"Switch vers : {characters[currentIndex].gameObject.name}");
    }
    
    private void ActivateCharacter(int index)
    {
        characters[index].GetComponent<PlayerStates>().isActiveCharacter = true;
        
        // (Optionnel) Ici, tu pourrais gérer le visuel : 
        // Par exemple, l'Artiste 1 pourrait vouloir que le perso inactif devienne transparent.
    }
}