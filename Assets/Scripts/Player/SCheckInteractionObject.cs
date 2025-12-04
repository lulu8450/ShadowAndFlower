using UnityEngine;

public class SCheckInteractionObject : MonoBehaviour
{
    public GameObject collisionObject;
    public GameObject triggerObject;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            collisionObject = collision.gameObject;

        if (collision.gameObject.layer == LayerMask.NameToLayer("TriggerInteractable"))
            triggerObject = collision.gameObject;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            collisionObject = null;

        if (collision.gameObject.layer == LayerMask.NameToLayer("TriggerInteractable"))
            triggerObject = null;
    }
}
