using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerFollower : MonoBehaviour
{
    PlayerStates ps;
    SPlayerMove pm;

    [SerializeField] Transform playerToFollow;

    [SerializeField] float distanceFollowOffset = 1.5f;
    [SerializeField] float maxDistance = 6f;
    [SerializeField] float speedMultiplicator = 0.9f;

    bool canShowOffset;

    [ShowIf(nameof(canShowOffset))]
    [SerializeField] float offset = 1.5f;

    private void Awake()
    {
        ps = GetComponent<PlayerStates>();
        pm = GetComponent<SPlayerMove>();

        canShowOffset = ps.persoType == PlayerStates.Perso.Shadow;
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

    PlayerStates FindAdesState()
    {
        PlayerStates[] allPlayers = FindObjectsByType<PlayerStates>(FindObjectsSortMode.None);

        if (transform != playerToFollow)
            foreach (var player in allPlayers)
                if (player.persoType == PlayerStates.Perso.Ades) return player;

        return null;
    }

    void FollowPlayer()
    {
        if (ps.persoType != PlayerStates.Perso.Shadow)
        {
            float distance = Vector3.Distance(transform.position, playerToFollow.position);

            if (distance <= distanceFollowOffset)
            {
                ps.isMoving = false;
                return;
            }

            ps.isMoving = true;

            float sprintSpeed = pm.GetSprintMultiplicator() * (pm.GetSpeed() * speedMultiplicator);
            float currentSpeed = distance > maxDistance ? sprintSpeed : (pm.GetSpeed() * speedMultiplicator);
            Vector3 direction = (playerToFollow.position - transform.position).normalized;

            transform.position += direction * currentSpeed * Time.fixedDeltaTime;

            pm.UpdateFacing(direction);

            if (distance > 25f)
            {
                transform.position = playerToFollow.position + new Vector3(1, 0, 1);
            }
        }
        else
        {
            PlayerStates playerAdes = FindAdesState();
            Vector3 direction = Vector3.zero;

            if (playerAdes.facing == PlayerStates.Facing.Left) direction = Vector3.left;
            if (playerAdes.facing == PlayerStates.Facing.Right) direction = Vector3.right;
            if (playerAdes.facing == PlayerStates.Facing.Face) direction = Vector3.forward;
            if (playerAdes.facing == PlayerStates.Facing.Back) direction = Vector3.back;

            transform.position = playerToFollow.position + (direction * offset);
        }
    }
}
