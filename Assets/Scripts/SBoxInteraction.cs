using UnityEngine;

public class SBoxInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] PlayerStates ps;

    public void OnInteractStart(SPlayerInteraction player)
    {
        Debug.Log("Intéraction avec la box !");
    }
}
