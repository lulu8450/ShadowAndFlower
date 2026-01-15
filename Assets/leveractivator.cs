using UnityEngine;

public class leveractivator : MonoBehaviour
{
    [SerializeField] bool isActivate;
    [SerializeField] Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.StopPlayback();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            animator.Play("LevierOn");
        }
        else 
        {
            animator.Play("LevierIdle");
        }
    }
}
