using UnityEngine;

public class LeverStates : MonoBehaviour
{
    [Header("State")]
    public bool isActive;

    public void SwitchState() => isActive = !isActive;
}
