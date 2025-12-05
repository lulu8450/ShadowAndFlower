using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    PlayerStates ps;
    SPlayerMove pm;

    [SerializeField] Transform playerToFollow;

    [SerializeField] float distanceOffset = 1.5f;
    [SerializeField] float maxDistance = 6f;
    [SerializeField] float speedMultiplicator = 0.8f;

    private void Awake()
    {
        ps = GetComponent<PlayerStates>();
        pm = GetComponent<SPlayerMove>();
    }

    private void FixedUpdate()
    {
        if (ps.canFollow)
        {
            // Find le Player non actif
            if (playerToFollow == null)
            {
                playerToFollow = FindActivePlayer();
                if (playerToFollow == null) return;
            }

            FollowPlayer();
        }
    }

    Transform FindActivePlayer()
    {
        PlayerStates[] allPlayers = FindObjectsByType<PlayerStates>(FindObjectsSortMode.None);

        if (transform != playerToFollow) 
            foreach (var player in allPlayers)
                if (player != ps && player.isActiveCharacter) return player.transform;

        return null;
    }

    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, playerToFollow.position);

        if (distance <= distanceOffset)
        {
            ps.isMoving = false;
            return;
        }

        ps.isMoving = true;

        float currentSpeed = distance > maxDistance ? pm.GetSprintMultiplicator() * (pm.GetSpeed() * speedMultiplicator) : (pm.GetSpeed() * speedMultiplicator);
        Vector3 direction = (playerToFollow.position - transform.position).normalized;

        transform.position += direction * currentSpeed * Time.fixedDeltaTime;

        pm.UpdateFacing(direction);

        if (distance > 25f)
        {
            transform.position = playerToFollow.position + new Vector3(1, 0, 1);
        }
    }
}
