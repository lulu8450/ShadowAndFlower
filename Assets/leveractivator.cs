using UnityEngine;

public class leveractivator : MonoBehaviour
{
    public bool isActivate = false;
    public bool playerNear;
    [SerializeField] PlayerStates playerStates;
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
        if (Input.GetKeyDown(KeyCode.E) && !isActivate && playerNear)
        {
            isActivate = true;
            Debug.Log($"Levier {gameObject.name} Activé!");
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Trigger Enter!");
            playerNear = true;     
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Trigger Exit!");
            collision.GetComponent<PlayerStates>().LockTriggerInteraction();
            playerNear = false;
        }
    }
}
