using UnityEngine;
using UnityEngine.Events; // Pour notifier d'autres systèmes

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // État actuel du jeu (commence en Exploration)
    [SerializeField] private GameState currentState = GameState.Exploration;
    
    // Événement pour notifier les autres scripts du changement d'état
    public UnityEvent<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Optionnel : ne pas détruire entre les scènes
            DontDestroyOnLoad(gameObject); 
        }
    }

    /// <summary>
    /// Change l'état actuel du jeu et notifie les systèmes concernés.
    /// </summary>
    public void UpdateGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged.Invoke(newState);
        
        Debug.Log($"État du jeu changé : {newState}");

        // Le Dév 2 utilisera cet événement pour désactiver les inputs de mouvement.
    }
    
    public GameState GetCurrentState()
    {
        return currentState;
    }
}