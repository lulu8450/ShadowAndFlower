using UnityEngine;
using UnityEngine.InputSystem;

public class SPlayerMove : MonoBehaviour
{
    InputSystemActions inputActions;
    Rigidbody rb;

    PlayerStates ps;

    [SerializeField] Transform sprite;

    [Header("Move Settings")]
    [SerializeField] float speed;

    [Header("Sprint Settings")]
    [SerializeField] float sprintMultiplier;

    [Header("Rotation Sprite Settings")]
    [SerializeField] float rotateDuration = 0.2f;
    float rotateTimer = 0f;
    bool rotating = false;
    Quaternion rotateStart;
    Quaternion rotateEnd;

    Vector2 moveInput;
    
    [Header("Flower Trail Settings")]
    [SerializeField] GameObject[] flowerPrefabs; // Liste des Prefabs de fleurs
    [SerializeField] float spawnRate = 0.2f; // Taux d'apparition
    [SerializeField] float spawnDistanceOffset = 0.5f; // Distance derri�re le player
    float spawnTimer;

    [Header("ground Placement")]
    [SerializeField] float raycastDistance = 2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float heightOffset = 0.05f;

    private void OnEnable() => inputActions.Enable();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        ps = GetComponent<PlayerStates>();

        inputActions = new InputSystemActions();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        
        // Initiliser le timer
        spawnTimer = spawnRate;
    }

    private void Update()
    {
        if (rotating)
        {
            rotateTimer += Time.deltaTime;
            float t = rotateTimer / rotateDuration;

            sprite.rotation = Quaternion.Slerp(rotateStart, rotateEnd, t);

            if (t >= 1f)
            {
                rotating = false;
                sprite.rotation = rotateEnd;
            }
        }

        // Mettre � jour le timer dans Update pour l'ind�pendance de la physique
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //TODO : Changer le system de mouvement pour la camera
        if (ps.canMove)
        {
            float currentSpeed = ps.isSprinting ? speed * sprintMultiplier : speed;
            Vector3 worldDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
            Vector3 velocity = worldDirection * currentSpeed;

            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

            UpdateFacing(worldDirection);

            //Cr�ation de Fleurs
        if (ps.isMoving)
            {
                TrySpawnFlower(worldDirection);
            }

        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero) ps.isMoving = true;
        else ps.isMoving = false;
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        if (ps.canSprint)
        {
            if (context.performed) ps.isSprinting = true;
            else if (context.canceled) ps.isSprinting = false;
        }
    }

    void RotateSprite(Vector3 direction)
    {
        float targetY = direction.x > 0 ? 0f : 180f;

        rotating = true;
        rotateTimer = 0f;
        rotateStart = sprite.rotation;
        rotateEnd = Quaternion.Euler(0f, targetY, 0f);
    }

    public void UpdateFacing(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            if (Mathf.Abs(direction.x) > 0.05f)
            {
                if (direction.x > 0) ps.facing = PlayerStates.Facing.Right;
                else ps.facing = PlayerStates.Facing.Left;

                RotateSprite(direction);
            }

            if (Mathf.Abs(direction.z) > Mathf.Abs(direction.x))
            {
                if (direction.z > 0) ps.facing = PlayerStates.Facing.Face;
                else ps.facing = PlayerStates.Facing.Back;
            }
        }
    }

    public float GetSpeed() => speed;
    public float GetSprintMultiplicator() => sprintMultiplier;

    // Ajout de la m�thode pour les fleurs
    void TrySpawnFlower(Vector3 moveDirection)
    {
        if (flowerPrefabs.Length == 0) return;

        // Si timer est �coul�
        if (spawnTimer <= 0)
        {
            // 1. Calculer la position d'apparition th�orique (derri�re le joueur)
            Vector3 spawnOffset = -moveDirection.normalized * spawnDistanceOffset;
            // On monte la position initiale pour que le Raycast puisse trouver le sol en dessous
            Vector3 raycastStart = transform.position + spawnOffset + Vector3.up * raycastDistance;
            Vector3 spawnPosition = Vector3.zero;

            RaycastHit hit;

            // 2. Lancer un Raycast vers le bas
            // La position de d�part est l�g�rement au-dessus de la position du joueur + l'offset,
            // et on tire vers le bas (-Vector3.up)
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, raycastDistance * 2, groundLayer))
            {
                // Le Raycast a touch� le sol !
                // La position de la fleur est le point de contact + un petit offset en Y.
                spawnPosition = hit.point + Vector3.up * heightOffset;
            }
            else
            {
                // Le Raycast n'a rien trouv� (joueur dans le vide ?), on annule le spawn.
                Debug.LogWarning("Impossible de trouver le sol pour placer la fleur. V�rifiez la Layer Mask.");
                return;
            }


            // 3. Choix d'une fleur au hasard
            GameObject randomFlowerPrefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];

            // 4. Instancier la fleur � la position du sol trouv�e
            // On utilise la rotation de l'impact du Raycast pour aligner la fleur au sol (si sol inclin�)
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            GameObject flower = Instantiate(randomFlowerPrefab, spawnPosition, spawnRotation);

            // 5. (Votre logique existante pour le script FlowersTrail)
            FlowersTrail flowerTrail = flower.GetComponent<FlowersTrail>();

            if (flowerTrail == null)
            {
                flowerTrail = flower.AddComponent<FlowersTrail>();
            }

            // 6. R�initialiser le timer
            spawnTimer = spawnRate;
        }
    }
}
