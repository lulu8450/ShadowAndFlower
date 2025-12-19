using System.Collections;
using UnityEngine;

public class BrigdeTriggerInteraction : MonoBehaviour, ITriggerInteractable
{  
    [SerializeField] GrowVines bridgeVines;
    [SerializeField] GameObject bridgeCollider;
    [SerializeField] Transform visualEffect;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] DialogueData brokenBridgeDialogueData;
    [SerializeField] DialogueData repairBridgeDialogueData;
    private PlayerStates playerStates;

    private void Start() {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
    }
    public void OnInteractStart(SPlayerInteraction player)
    {
        StartCoroutine(powerAnimation());
        
    }

    public IEnumerator powerAnimation()
    {
        bridgeVines.isGrowingOrRetracting = true;
        bridgeVines.isFullyGrown = !bridgeVines.isFullyGrown;
        StartCoroutine(bridgeVines.GrowVineRoutine(bridgeVines.isFullyGrown));
        yield return new WaitUntil(() => bridgeVines.isGrowingOrRetracting == false);
        dialogueManager.StartDialogue(repairBridgeDialogueData);
        bridgeVines.playerIsClose = false;
        playerStates.LockTriggerInteraction();
        bridgeCollider.SetActive(false);
    }

    IEnumerator dialogueDelay()
    {
        dialogueManager.StartDialogue(brokenBridgeDialogueData);
        yield return new WaitUntil(() => dialogueManager.dialogueIsComplete == true);
        if (dialogueManager.dialogueIsComplete) bridgeVines.playerIsClose = true;
        playerStates.LockMovement();
        playerStates.DeLockTriggerInteraction();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStates = collision.GetComponent<PlayerStates>();
            playerStates.LockInteraction();
            playerStates.LockTriggerInteraction();
            StartCoroutine(dialogueDelay());
            visualEffect.position = new Vector3(collision.transform.position.x, visualEffect.position.y, collision.transform.position.z);
        }
    }
}
