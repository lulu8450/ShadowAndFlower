using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BrigdeTriggerInteraction : MonoBehaviour, ITriggerInteractable
{
    [SerializeField] Transform bridge;
    [SerializeField] GameObject bridgeColliderCenter;

    public void OnInteractStart(SPlayerInteraction player)
    {
        // A changer
        bridge.position = new Vector3(bridge.position.x, 0, bridge.position.z);
        // ------------------------

        bridgeColliderCenter.SetActive(false);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
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
