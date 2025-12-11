using UnityEngine;

public class BrigdeTriggerInteraction : MonoBehaviour, ITriggerInteractable
{
    [SerializeField] Transform bridge;
    [SerializeField] GameObject bridgeColliderCenter;

    public void OnInteractStart(SPlayerInteraction player)
    {
        // A changer
        Debug.Log("Trigger Interaction in interactable");
        // ------------------------

        bridgeColliderCenter.SetActive(false);

        player.GetComponent<PlayerStates>().LockTriggerInteraction();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Debug.Log("Trigger Interaction in trigger");
            collision.GetComponent<PlayerStates>().DeLockTriggerInteraction();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerStates>().LockTriggerInteraction();
        }
    }
}
