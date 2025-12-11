using UnityEngine;

public class LeverTriggerInteraction : MonoBehaviour, ITriggerInteractable
{
    [SerializeField] LeverStates ls;
    [SerializeField] Transform leverMesh;

    public void OnInteractStart(SPlayerInteraction player)
    {
        ls.SwitchState();

        // A changer
        if (ls.isActive) leverMesh.rotation = Quaternion.Euler(new Vector3(leverMesh.rotation.x, leverMesh.rotation.y, -90));
        else leverMesh.rotation = Quaternion.Euler(new Vector3(leverMesh.rotation.x, leverMesh.rotation.y, 0)); ;
        // ------------------------

        Debug.Log("Interaction!");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerStates>().DeLockTriggerInteraction();
            Debug.Log("Trigger Enter!");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerStates>().LockTriggerInteraction();
            Debug.Log("Trigger Exit!");
        }
    }
}
