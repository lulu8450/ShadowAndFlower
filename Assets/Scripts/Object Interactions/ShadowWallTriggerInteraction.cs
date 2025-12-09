using System.Collections;
using UnityEngine;

public class ShadowWallTriggerInteraction : MonoBehaviour, ITriggerInteractable
{
    [SerializeField] Transform wall;

    [SerializeField] float dashForce = 20f;
    [SerializeField] float dashDuration = 0.15f;

    public void OnInteractStart(SPlayerInteraction player)
    {
        PlayerStates ps = player.GetComponent<PlayerStates>();

        if (ps.canDash || !ps.isDashing)
            StartCoroutine(Dash(player));

        Debug.Log("J'ai mal...");
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Test 1");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Test 2");
            PlayerStates player = collision.GetComponent<PlayerStates>();

            if (player.persoType == PlayerStates.Perso.Shadow)
            {
                Debug.Log("Test 3");
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

    IEnumerator Dash(SPlayerInteraction player)
    {
        PlayerStates ps = player.GetComponent<PlayerStates>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        ps.isDashing = true;
        rb.useGravity = false;

        Vector3 dashDirection = Vector3.forward;

        if (ps.facing == PlayerStates.Facing.Left) dashDirection = Vector3.left;
        if (ps.facing == PlayerStates.Facing.Right) dashDirection = Vector3.right;
        if (ps.facing == PlayerStates.Facing.Face) dashDirection = Vector3.forward;
        if (ps.facing == PlayerStates.Facing.Back) dashDirection = Vector3.back;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
        ps.isDashing = false;
    }
}
