using UnityEngine;

public class PuzzleRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStates ps = other.gameObject.GetComponent<PlayerStates>();

            //if (!ps.isActiveCharacter && ps.persoType != PlayerStates.Perso.Shadow)
            //{
            //    ps.canFollow = false;
            //}
            
            //ps.following = PlayerStates.Following.Stay;

            if (ps.persoType != PlayerStates.Perso.Shadow)
            {
                ps.following = PlayerStates.Following.Stay;
                ps.canFollow = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStates[] allPlayers = FindObjectsByType<PlayerStates>(FindObjectsSortMode.None);

            foreach (PlayerStates ps in allPlayers)
            {
                if (ps.persoType != PlayerStates.Perso.Shadow)
                {
                    ps.following = PlayerStates.Following.Follow;

                    if (!ps.isActiveCharacter) ps.canFollow = true;
                }
            }
        }
    }
}
