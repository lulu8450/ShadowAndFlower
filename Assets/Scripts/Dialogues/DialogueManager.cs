using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    // Référence aux éléments UI (à assigner dans l'Inspector)
    [Header("UI References")]
    public Canvas dialogueCanvas; // Le canvas de dialogue
    public GameObject dialoguePanel; // Le panneau de dialogue
    public Image imageSpeaker; // Image du personnage qui parle
    public TextMeshProUGUI canPassText; // Texte indiquant que le joueur peut passer
    public TextMeshProUGUI nameText; // Le nom du personnage qui parle
    public TextMeshProUGUI dialogueText; // Le texte de dialogue
    public Transform choicesParent; // Le parent des boutons de choix
    public GameObject choiceButtonPrefab; // La prefab du bouton de choix
    public int currentIndex = 0; // Index pour suivre la phrase actuelle
    [SerializeField] private Coroutine typingCoroutine; // référence à la coroutine de frappe de texte
    [SerializeField] private bool isTyping = false; // Indique si le texte est en train d'être tapé ou non
    [SerializeField] public float typingSpeed = 0.05f; // Délai entre chaque lettre
    [SerializeField] private DialogueData currentDialogue; // Le noeud de dialogue actuel
    [SerializeField] private LinkManager linkManager; // Référence au système de lien émotionnel (à implémenter)
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private bool next = false;
    [SerializeField] private InputActionReference interactAction; 
    public bool dialogueIsComplete = false;

    public void StartDialogue(DialogueData startNode)
    {
        if (playerStates != null) playerStates.LockMovement();
        if (choicesParent != null) choicesParent.gameObject.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(true);

        // Initialisation de la conversation
        currentDialogue = startNode;
        currentIndex = 0;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Dialogue);
        }
        linkManager = LinkManager.Instance;
        DisplayDialogue();
    }
    private void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            interactAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // Only set next if player is allowed to interact
        if (playerStates == null || playerStates.canInteract)
        {
            next = true;
        }
    }

    private void DisplayDialogue()
    {
        if (currentDialogue == null)
        {
            EndDialogue();
            return;
        }
        // 1. Afficher le nom et la phrase
        // Use the speakerName field from DialogueData
        if (nameText != null) nameText.text = currentDialogue.speakerName;
        // Mettre à jour l'image du personnage qui parle
        if (imageSpeaker != null) imageSpeaker.sprite = currentDialogue.sprite;

        // Démarrer la Coroutine de frappe de texte
        // Si une coroutine est déjà en cours (clic rapide du joueur), on l'arrête
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        // Lancer la coroutine pour le déroulement du texte (une phrase à la fois)
        typingCoroutine = StartCoroutine(TypeSentence());

        // 2. Gérer les choix ou la progression automatique
        if (currentDialogue.hasChoices)
        {
            // C'est une Joute Verbale! Afficher les choix.
            DisplayChoices(currentDialogue);
        }
        else
        {
            // TODO : Logique d'attente/input pour passer au dialogue suivant
            // StartCoroutine(WaitForPlayerInteraction());
        }
    }

    public void ProgressDialogue()
    {
        if (currentDialogue == null) return;

        // If text is typing, finish current sentence
        if (isTyping)
        {
            CompleteCurrentSentence();
            return;
        }

        // If there are more sentences in this node, advance to next
        currentIndex++;
        if (currentDialogue.sentences != null && currentIndex < currentDialogue.sentences.Count)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeSentence());
            return;
        }

        // No more sentences in current node
        if (currentDialogue.hasChoices)
        {
            DisplayChoices(currentDialogue);
            return;
        }

        // Move to next dialogue node
        currentDialogue = currentDialogue.nextDialogue;
        currentIndex = 0;
        if (currentDialogue != null)
        {
            DisplayDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void CompleteCurrentSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null && currentDialogue != null && currentDialogue.sentences != null &&
            currentIndex >= 0 && currentIndex < currentDialogue.sentences.Count)
        {
            dialogueText.text = currentDialogue.sentences[currentIndex];
        }

        isTyping = false;
    }
    private void DisplayChoices(DialogueData nodeWithChoices)
    {
        // Nettoyer les anciens choix
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }
        choicesParent.gameObject.SetActive(true);
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
        if (LinkManager.Instance != null)
        {
            LinkManager.Instance.UpdateLink(selectedChoice.linkImpact); 
        }

        // Nettoyer les boutons de choix
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }

        // Passer au dialogue suivant
        currentDialogue = selectedChoice.nextDialogue;
        DisplayDialogue();
    }

    private void EndDialogue() // Logique de fin de conversation (cacher l'UI, déclencher un événement de gameplay, etc.)
    {
        // Rétablir l'état du jeu à l'exploration
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Exploration);
        }
        dialogueCanvas.gameObject.SetActive(false);
        playerStates.DeLockMovement();
        dialogueIsComplete = true;
        Debug.Log("Fin de la séquence de dialogue.");
    }

    // Nouvelle Coroutine pour le déroulement
    IEnumerator TypeSentence()
    {
        if (currentDialogue == null || currentDialogue.sentences == null || currentIndex >= currentDialogue.sentences.Count)
        {
            yield break;
        }

        isTyping = true;
        string sentence = currentDialogue.sentences[currentIndex];
        if (dialogueText != null) dialogueText.text = ""; // Initialiser le texte à vide

        foreach (char letter in sentence.ToCharArray())
        {
            if (dialogueText != null) dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Finished typing one sentence
        isTyping = false;

        // If current node has choices, show them immediately
        if (currentDialogue.hasChoices)
        {
            DisplayChoices(currentDialogue);
            yield break;
        }

        // Otherwise wait for player interaction before proceeding
        yield return StartCoroutine(WaitForPlayerInteraction());

        // Advance to next sentence or node
        currentIndex++;
        if (currentDialogue != null && currentDialogue.sentences != null && currentIndex < currentDialogue.sentences.Count)
        {
            typingCoroutine = StartCoroutine(TypeSentence());
            yield break;
        }

        // No more sentences in this node
        if (currentDialogue != null && currentDialogue.nextDialogue != null)
        {
            currentDialogue = currentDialogue.nextDialogue;
            currentIndex = 0;
            DisplayDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    // Fonction pour gérer le clic du joueur
    public void HandlePlayerInput()
    {
        if (currentDialogue == null) return;

        if (isTyping)
        {
            // Finish typing immediately
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                if (dialogueText != null && currentDialogue.sentences != null && currentIndex < currentDialogue.sentences.Count)
                    dialogueText.text = currentDialogue.sentences[currentIndex];
                isTyping = false;
            }
        }
        else
        {
            // Trigger the same flag as input action
            next = true;
        }
        // If hasChoices is True, the click does nothing other than allowing selection
    }

    private IEnumerator WaitForPlayerInteraction()
    {
        // Small initial delay to avoid instant skipping
        yield return new WaitForSeconds(0.2f);

        if (canPassText != null) canPassText.text = "Press E to pass";
        if (playerStates != null) playerStates.canInteract = true;

        // Wait until the interact action sets next to true
        yield return new WaitUntil(() => next == true);

        if (canPassText != null) canPassText.text = "";
        next = false;
        if (playerStates != null) playerStates.canInteract = false;
    }
}
