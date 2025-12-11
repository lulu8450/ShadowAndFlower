using UnityEngine;

public class NimpheTrigger : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] DialogueData dialogueData;
    [SerializeField] BoxCollider boxCollider;
    private void Start() {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(boxCollider);
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}
