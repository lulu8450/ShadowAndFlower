using UnityEngine;
using UnityEngine.Events;

public class LinkManager : MonoBehaviour
{
    // Singleton pour accès facile depuis DialogueManager, etc.
    public static LinkManager Instance { get; private set; }

    [Header("Paramètres de Lien")]
    [Tooltip("Valeur actuelle du lien (0 = Neutre/Distance, 100 = Union/Confiance).")]
    [SerializeField] private float currentLinkValue = 50f; 
    public float maxLinkValue = 100f;
    public float minLinkValue = 0f;
    
    public UnityEvent<float> OnLinkUpdated; // Événement déclenché lorsque le lien change (pour l'UI discrète du Dév 2)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garder le lien entre les scènes
        }
    }

    /// <summary>
    /// Met à jour la valeur du lien en fonction du choix de dialogue.
    /// </summary>
    /// <param name="impact">Impact positif ou négatif du choix sur le lien (pris de Choice.linkImpact).</param>
    public void UpdateLink(float impact)
    {
        currentLinkValue += impact;
        
        // S'assurer que la valeur reste dans les limites définies (0-100)
        currentLinkValue = Mathf.Clamp(currentLinkValue, minLinkValue, maxLinkValue);

        // Déclencher l'événement pour que l'UI (Dév 2) ou d'autres systèmes réagissent
        OnLinkUpdated.Invoke(currentLinkValue);

        Debug.Log($"Lien mis à jour par {impact}. Nouvelle valeur : {currentLinkValue}");
    }
    
    /// <summary>
    /// Vérifie si le lien atteint le seuil requis pour débloquer une option.
    /// Nécessaire pour le Chapitre 10 (Le Dernier Choix).
    /// </summary>
    public bool CheckLinkThreshold(float requiredValue)
    {
        return currentLinkValue >= requiredValue;
    }
    
    public float GetCurrentLinkValue()
    {
        return currentLinkValue;
    }
}