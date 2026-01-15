using UnityEngine;

public class camSwitch : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger(triggerName);
        }
    }
}
