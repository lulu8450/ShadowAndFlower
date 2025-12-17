using System.Collections;
using UnityEngine;

public class BrigdeTriggerInteraction : MonoBehaviour, ITriggerInteractable
{  
    [SerializeField] GrowVines bridgeVines;
    [SerializeField] GameObject bridgeCollider;
    private PlayerStates playerStates;

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
        bridgeVines.playerIsClose = false;
        playerStates.DeLockMovement();
        playerStates.LockTriggerInteraction();
        bridgeCollider.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStates = collision.GetComponent<PlayerStates>();
            // Debug.Log("Trigger Interaction in trigger");
            bridgeVines.playerIsClose = true;
            playerStates.LockMovement();
            playerStates.DeLockTriggerInteraction();
        }
    }
}
