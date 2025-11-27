using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Référence aux éléments UI (à assigner dans l'Inspector)
    [Header("UI References")]
    public TextMeshProUGUI nameText; // Le nom du personnage qui parle
    public TextMeshProUGUI dialogueText; // Le texte de dialogue
    public Transform choicesParent; // Le parent des boutons de choix
    public GameObject choiceButtonPrefab; // La prefab du bouton de choix

    [SerializeField] private Coroutine typingCoroutine; // référence à la coroutine de frappe de texte
    [SerializeField] private bool isTyping = false; // Indique si le texte est en train d'être tapé ou non
    [SerializeField] public float typingSpeed = 0.05f; // Délai entre chaque lettre
    [SerializeField] private DialogueData currentDialogue; // Le noeud de dialogue actuel
    [SerializeField] private LinkManager linkManager; // Référence au système de lien émotionnel (à implémenter)

    public void StartDialogue(DialogueData startNode)
    {
        // Initialisation de la conversation
        currentDialogue = startNode;
        linkManager = LinkManager.Instance;
        DisplayDialogue();
    }

    private void DisplayDialogue()
    {
        if (currentDialogue == null)
        {
            EndDialogue();
            return;
        }

        // 1. Afficher le nom et la phrase
        nameText.text = currentDialogue.speakerName;
        // Démarrer la Coroutine de frappe de texte
        // Si une coroutine est déjà en cours (clic rapide du joueur), on l'arrête
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        // Lancer la coroutine pour le déroulement du texte
        typingCoroutine = StartCoroutine(TypeSentence(currentDialogue.sentence));

        // 2. Gérer les choix ou la progression automatique
        if (currentDialogue.hasChoices)
        {
            // C'est une Joute Verbale! Afficher les choix.
            DisplayChoices(currentDialogue);
        }
        else
        {
            // TODO : Logique d'attente/input pour passer au dialogue suivant
        }
    }

    public void ProgressDialogue()
    {
        // Fonction appelée par un clic du joueur si pas de choix
        if (!currentDialogue.hasChoices)
        {
            currentDialogue = currentDialogue.nextDialogue;
            DisplayDialogue();
        }
        // Si c'est un choix, on ne progresse que via le bouton de choix
    }

    private void DisplayChoices(DialogueData nodeWithChoices)
    {
        // Nettoyer les anciens choix
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }

        // Créer les boutons de choix (Telle une Joute Verbale)
        foreach (var choice in nodeWithChoices.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            
            // Ajouter un Listener au clic du bouton
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private void OnChoiceSelected(Choice selectedChoice)
    {
        // Logique de la Joute : appliquer l'impact sur le lien émotionnel
        linkManager.UpdateLink(selectedChoice.linkImpact); 

        // Nettoyer les boutons de choix
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }

        // Passer au dialogue suivant
        currentDialogue = selectedChoice.nextDialogue;
        DisplayDialogue();
    }

    private void EndDialogue()
    {
        // Logique de fin de conversation (cacher l'UI, déclencher un événement de gameplay, etc.)
        Debug.Log("Fin de la séquence de dialogue.");
    }

    // Nouvelle Coroutine pour le déroulement
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; // Initialiser le texte à vide
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        
        // Si ce n'est pas un choix, le joueur peut maintenant cliquer pour progresser.
        if (!currentDialogue.hasChoices)
        {
            // Indicateur visuel (flèche clignotante, par exemple) pour avancer.
        }
    }

    // Fonction pour gérer le clic du joueur
    public void HandlePlayerInput()
    {
        if (currentDialogue == null) return;

        if (isTyping)
        {
            // Si le joueur clique pendant le déroulement : on affiche le texte complet immédiatement
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue.sentence;
                isTyping = false;
            }
        }
        else if (!currentDialogue.hasChoices)
        {
            // Si le texte est fini et pas de choix : on passe à la phrase suivante
            ProgressDialogue();
        }
        // Si hasChoices est True, le clic ne fait rien (il faut sélectionner un bouton)
    }
}
