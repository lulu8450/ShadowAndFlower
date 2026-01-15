using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.forward * v + transform.right * h).normalized;

        transform.position += move * speed * Time.deltaTime;
    }
}
