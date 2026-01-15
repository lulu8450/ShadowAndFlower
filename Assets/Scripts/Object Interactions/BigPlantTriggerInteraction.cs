using UnityEngine;

public class BigPlantTriggerInteraction : MonoBehaviour, ITriggerInteractable
{
    [SerializeField] GameObject plant;

    public void OnInteractStart(SPlayerInteraction player)
    {
        // A changer
        plant.transform.position = new Vector3(plant.transform.position.x, -2.25f, plant.transform.position.z);
        // ------------------------
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
