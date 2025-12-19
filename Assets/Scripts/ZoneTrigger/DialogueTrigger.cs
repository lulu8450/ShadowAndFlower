using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] DialogueData dialogueData;
    [SerializeField] BoxCollider boxCollider;
    private void Start() {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(boxCollider);
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}
