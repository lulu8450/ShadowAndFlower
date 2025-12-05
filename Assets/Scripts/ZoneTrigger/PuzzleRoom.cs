using UnityEngine;

public class PuzzleRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStates ps = other.gameObject.GetComponent<PlayerStates>();

            if (!ps.isActiveCharacter )
            {
                ps.canFollow = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStates ps = other.gameObject.GetComponent<PlayerStates>();

            if (!ps.isActiveCharacter && ps.canFollow) ps.canFollow = false;
            //else if ()
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStates ps = other.gameObject.GetComponent<PlayerStates>();

            if (!ps.isActiveCharacter)
            {
                ps.canFollow = true;
            }
        }
    }
}
