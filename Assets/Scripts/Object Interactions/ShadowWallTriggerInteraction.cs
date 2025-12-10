using System.Collections;
using UnityEngine;

public class ShadowWallTriggerInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStates player = collision.GetComponent<PlayerStates>();

            if (player.persoType == PlayerStates.Perso.Shadow)
            {
                player.DeLockTriggerInteraction();
                player.canDash = true;
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStates player = collision.GetComponent<PlayerStates>();

            if (player.persoType == PlayerStates.Perso.Shadow)
            {
                player.DeLockTriggerInteraction();
                player.canDash = false;
            }
        }
    }
}
