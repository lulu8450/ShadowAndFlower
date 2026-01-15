using UnityEngine;

public class colliderCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.name + " entered the collider of " + gameObject.name);
    }
}
