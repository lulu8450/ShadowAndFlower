//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.InputSystem;
//using UnityEngine.VFX;
//using Unity.VisualScripting;

//public class GrowVines : MonoBehaviour
//{
//    [Header("Settings")]
//    public List<MeshRenderer> growVinesMeshes;
//    public float timeToGrow = 5f;
//    public float refreshRate = 0.05f;

//    [Range(0, 1)]
//    public float minGrow = 0.0f;
//    [Range(0, 1)]
//    public float maxGrow = 1.0f;

//    [Header("Emissive")]
//    public float emissiveStrenght;
//    public int maxEmissiveStrenght = 100;
//    public int minEmissiveStrenght = 25;

//    [Header("VFX Integration")]
//    public VisualEffect vinesVFX;

//    [Header("Interaction Info (Debug)")]
//    [SerializeField] private bool playerIsClose = false; // Visible dans l'inspecteur pour v�rifier
//    [SerializeField] private Material gameObjectMaterial;
//    [SerializeField] private Material globalMaterial;

//    // �tat interne
//    private bool isGrowingOrRetracting = false;
//    private bool isFullyGrown = false;
//    private float currentGrowValue;

//    // Optimisation : PropertyBlock pour ne pas modifier le material partag�
//    private MaterialPropertyBlock _propBlock;

//    void Start()
//    {
//        gameObjectMaterial = new Material(globalMaterial);
//        // Initialisation du PropertyBlock
//        //_propBlock = new MaterialPropertyBlock();
//        if (isFullyGrown) {
//            currentGrowValue = maxGrow;
//        }
//        else {currentGrowValue = minGrow;}

//        // On applique les valeurs initiales (minGrow) � tous les meshes
//        UpdateMaterials(minGrow, maxEmissiveStrenght);

//        if (vinesVFX != null)
//        {
//            vinesVFX.Stop();
//        }
//    }

//    void Update()
//    {
//        // CONDITION AJOUT�E : On v�rifie si le joueur est proche (playerIsClose)
//        if (playerIsClose && Input.GetKeyDown(KeyCode.Space) && !isGrowingOrRetracting)
//        {
//            Debug.Log($"SpaceKey On: Starting {(isFullyGrown ? "Retraction" : "Growth")}");
//            isGrowingOrRetracting = true;
//            isFullyGrown = !isFullyGrown;

//            StartCoroutine(GrowVineRoutine(isFullyGrown));
//        }
//    }

//    // --- GESTION DE LA D�TECTION DU JOUEUR ---

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerIsClose = true;
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerIsClose = false;
//        }
//    }

//    // -----------------------------------------

//    // Fonction unique pour mettre � jour tous les meshes list�s sans toucher au fichier Material
//    private void UpdateMaterials(float growVal, float emissiveVal)
//    {
//        List<Material> mat = new List<Material>();
//        mat.Add(gameObjectMaterial);
//        Shader shader;

//        foreach (var mesh in growVinesMeshes)
//        {
//            Debug.Log("dfdgdfdgf");
//            shader = Shader.Find(mesh.name);
//            if (mesh == null) continue;


//            // 1. On r�cup�re les propri�t�s actuelles de l'objet
//            //mesh.GetPropertyBlock(_propBlock);
//            mesh.GetComponent<MeshRenderer>().SetMaterials(mat);
//            Debug.Log("je fait le premier pas");

//            // 2. On modifie les valeurs
//            shader.SetFloat("Grow_", growVal);
//            _propBlock.SetFloat("EmissiveStrength_", emissiveVal);
//            Debug.Log("Elle veut pas de moi");

//            // 3. On r�applique le bloc modifi� au MeshRenderer
//            mesh.SetPropertyBlock(_propBlock);
//            Debug.Log("je l'aime quand meme et je la veux");
//        }
//    }

//    IEnumerator GrowVineRoutine(bool growing)
//    {
//        float targetGrow = growing ? maxGrow : minGrow;
//        int targetEmissive = growing ? minEmissiveStrenght : maxEmissiveStrenght;

//        // D�marrage VFX
//        if (growing && vinesVFX != null) vinesVFX.Play();

//        // Boucle d'animation
//        while ((growing && currentGrowValue < targetGrow) || (!growing && currentGrowValue > targetGrow))
//        {
//            float direction = growing ? 1f : -1f;
//            currentGrowValue += direction * (1f / (timeToGrow / refreshRate)) * refreshRate;

//            // Clamp pour ne jamais d�passer les bornes
//            currentGrowValue = Mathf.Clamp(currentGrowValue, minGrow, maxGrow);

//            // Calcul de l'�mission
//            float t = (currentGrowValue - minGrow) / (maxGrow - minGrow);
//            if (!growing) t = 1 - t;
//            emissiveStrenght = Mathf.Lerp(maxEmissiveStrenght, minEmissiveStrenght, t);

//            // Mise � jour visuelle via PropertyBlock
//            UpdateMaterials(currentGrowValue, emissiveStrenght);

//            yield return new WaitForSeconds(refreshRate);
//        }

//        // Arr�t VFX
//        if (growing && vinesVFX != null) vinesVFX.Stop();

//        // Valeurs finales exactes pour �viter les petits d�calages
//        UpdateMaterials(targetGrow, targetEmissive);

//        isGrowingOrRetracting = false;
//    }
//}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using TMPro;

public class GrowVines : MonoBehaviour
{
    [Header("Settings")]
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5f;
    public float refreshRate = 0.05f;

    [Range(0, 1)]
    public float minGrow = 0.0f;
    [Range(0, 1)]
    public float maxGrow = 1.0f;

    [Header("Emissive")]
    public float emissiveStrenght;
    public int maxEmissiveStrenght = 100;
    public int minEmissiveStrenght = 25;

    [Header("VFX Integration")]
    public VisualEffect vinesVFX;
    public TextMeshProUGUI interactionPromptText;

    [Header("Interaction Info (Debug)")]
    public bool playerIsClose = false; // Vrai si le joueur est dans le Trigger

    // Instance du Material unique � cet objet (modifi�e � l'ex�cution)
    [SerializeField] private Material gameObjectMaterial;

    // Le Material de base (l'asset) � copier. Doit �tre assign� dans l'Inspecteur !
    [SerializeField] private Material globalMaterial;

    // �tat interne
    public bool isGrowingOrRetracting = false;
    public bool isFullyGrown = false;
    public bool caninteract = true;
    private float currentGrowValue;

    void Start()
    {
        // V�RIFICATION CRITIQUE : Cr�er une instance unique du material.
        if (globalMaterial != null)
        {
            gameObjectMaterial = new Material(globalMaterial);
        }
        else
        {
            Debug.LogError("Le Global Material n'est pas assign� ! Veuillez glisser l'asset Material dans le champ 'Global Material' dans l'Inspecteur.");
            enabled = false; // D�sactiver le script s'il ne peut pas fonctionner
            return;
        }

        // Assigner l'instance de material unique � tous les MeshRenderers
        foreach (var mesh in growVinesMeshes)
        {
            if (mesh != null)
            {
                // On remplace le material partag� par notre instance unique
                mesh.sharedMaterial = gameObjectMaterial;
            }
        }

        if (isFullyGrown)
        {
            currentGrowValue = maxGrow;
        }
        else { currentGrowValue = minGrow; }

        // Initialisation des valeurs du shader
        UpdateMaterials(currentGrowValue, maxEmissiveStrenght);

        if (vinesVFX != null)
        {
            vinesVFX.Stop();
        }
    }

    void Update()
    {
        // Le script se déclenche UNIQUEMENT si le joueur est dans la zone (Trigger)
        if (playerIsClose && Input.GetKeyDown(KeyCode.Space) && !isGrowingOrRetracting && caninteract)
        {
            Debug.Log($"SpaceKey On: Starting {(isFullyGrown ? "Retraction" : "Growth")} for {gameObject.name}");
            isGrowingOrRetracting = true;
            caninteract = false;
            interactionPromptText.text = "";
            isFullyGrown = !isFullyGrown;

            StartCoroutine(GrowVineRoutine(isFullyGrown));
        }
    }

    // --- GESTION DE LA D�TECTION DU JOUEUR ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            if (interactionPromptText != null && caninteract)
            {
                
                interactionPromptText.text = "Press E";
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            if (interactionPromptText != null)
            {
                interactionPromptText.text = "";
            }
        }
    }

    // -----------------------------------------

    // Fonction unique pour mettre � jour les propri�t�s sur l'instance de Material unique
    private void UpdateMaterials(float growVal, float emissiveVal)
    {
        var mesh = growVinesMeshes[0];
        // *******************************************************************
        // ATTENTION : Les noms des propri�t�s doivent correspondre EXACTEMENT 
        // au "Reference Name" de ton Shader Graph. (Ex: "Grow" ou "_Grow")
        // *******************************************************************

        if (gameObjectMaterial == null) return;

        // Modification de la valeur Grow
        // J'utilise "Grow" car c'est le label principal, si �a ne marche pas, essaie "_Grow"
        if (gameObjectMaterial.HasProperty("Grow_"))
        {
            Debug.Log("j'ai la propri�t� grow_");
            gameObjectMaterial.SetFloat("Grow_", growVal);
        }
        if (gameObjectMaterial.HasProperty("Grow"))
        {
            Debug.Log("j'ai la propri�t� grow");
            gameObjectMaterial.SetFloat("Grow", growVal);
        }

        // Modification de la force �missive
        // J'utilise "EmissiveStrength" comme dans ton image
        if (gameObjectMaterial.HasProperty("EmissiveStrength_"))
        {
            Debug.Log("j'ai la propri�t� EmissiveStrength_");
            gameObjectMaterial.SetFloat("EmissiveStrength_", emissiveVal);
        }

        // C'est tout. Le Material est modifi�, et Unity met � jour le MeshRenderer qui l'utilise.
    }

    public IEnumerator GrowVineRoutine(bool growing)
    {
        float targetGrow = growing ? maxGrow : minGrow;
        int targetEmissive = growing ? minEmissiveStrenght : maxEmissiveStrenght;
        Debug.Log($"growing is {growing}");

        // D�marrage VFX
        if (growing && vinesVFX != null) vinesVFX.Play();

        // Boucle d'animation
        while ((growing && currentGrowValue < targetGrow) || (!growing && currentGrowValue > targetGrow))
        {
            float direction = growing ? 1f : -1f;
            currentGrowValue += direction * (1f / (timeToGrow / refreshRate)) * refreshRate;

            // Clamp pour ne jamais d�passer les bornes
            currentGrowValue = Mathf.Clamp(currentGrowValue, minGrow, maxGrow);

            // Calcul de l'�mission (diminue pendant la croissance)
            float t = (currentGrowValue - minGrow) / (maxGrow - minGrow);
            if (!growing) t = 1 - t;
            emissiveStrenght = Mathf.Lerp(maxEmissiveStrenght, minEmissiveStrenght, t);

            // Mise � jour visuelle du Material unique
            UpdateMaterials(currentGrowValue, emissiveStrenght);

            yield return new WaitForSeconds(refreshRate);
        }

        // Arr�t VFX
        if (growing && vinesVFX != null) vinesVFX.Stop();

        // Valeurs finales exactes pour �viter les petits d�calages
        UpdateMaterials(targetGrow, targetEmissive);

        isGrowingOrRetracting = false;
    }
}