using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.VFX; 


public class GrowVines : MonoBehaviour
{
    public List<MeshRenderer> growVinesMeshes;
    public float timeToGrow = 5f;
    public float refreshRate = 0.05f;
    [Range(0, 1)]
    public float minGrow = 0.2f;
    [Range(0, 1)]
    public float maxGrow = 0.97f;

    public List<Material> growVinesMaterials = new List<Material>();
    // Attention: Je n'utilise pas fullyGrown comme booléen direct pour contrôler la direction, 
    // mais plutôt un booléen de contrôle pour éviter les démarrages multiples de coroutines.
    private bool isGrowingOrRetracting = false;

    [Header("EmissiveStrenght")]
    public float emissiveStrenght;
    public int maxEmissiveStrenght = 100;
    public int minEmissiveStrenght = 25;

    [Header("VFX Integration")]
    // Assurez-vous d'assigner ici votre système de particules VFX Graph
    public VisualEffect vinesVFX;
    private bool isFullyGrown = false; // L'état réel du mesh

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Correction de la boucle (le j doit être < Length, pas i > Count)
        for (int i = 0; i < growVinesMeshes.Count; i++)
        {
            for (int j = 0; j < growVinesMeshes[i].materials.Length; j++)
            {
                if (growVinesMeshes[i].materials[j].HasProperty("Grow_"))
                {
                    growVinesMeshes[i].materials[j].SetFloat("Grow_", minGrow);
                    growVinesMeshes[i].materials[j].SetFloat("EmissiveStrength_", maxEmissiveStrenght);
                    growVinesMaterials.Add(growVinesMeshes[i].materials[j]);
                }
            }
        }

        // Initialisation: Arrêter le VFX au début
        if (vinesVFX != null)
        {
            vinesVFX.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isGrowingOrRetracting)
        {
            Debug.Log($"SpaceKey On: Starting {(isFullyGrown ? "Retraction" : "Growth")}");

            // On inverse l'état et on lance la coroutine pour tous les matériaux
            isGrowingOrRetracting = true;
            isFullyGrown = !isFullyGrown;

            foreach (Material mat in growVinesMaterials)
            {
                StartCoroutine(GrowVine(mat, isFullyGrown));
            }
        }
    }

    IEnumerator GrowVine(Material mat, bool growing)
    {
        float growValue = mat.GetFloat("Grow_");
        float targetGrow = growing ? maxGrow : minGrow;
        int targetEmissive = growing ? minEmissiveStrenght : maxEmissiveStrenght;

        // ****************************************
        // 1. DÉMARRER LES PARTICULES (au début de la croissance)
        // ****************************************
        if (growing && vinesVFX != null)
        {
            vinesVFX.Play();
        }

        while ((growing && growValue < targetGrow) || (!growing && growValue > targetGrow))
        {
            float direction = growing ? 1f : -1f;
            growValue += direction * 1 / (timeToGrow / refreshRate);
            growValue = Mathf.Clamp(growValue, minGrow, maxGrow); // Pour éviter le dépassement

            mat.SetFloat("Grow_", growValue);

            // Gestion de la force émissive: l'émission diminue pendant la croissance et augmente pendant la rétraction.
            float t = (growValue - minGrow) / (maxGrow - minGrow); // 0 à 1 pour la croissance

            if (!growing) t = 1 - t; // 0 à 1 pour la rétraction (inverse)

            // L'émission est forte quand le mesh est petit (au début de la croissance ou à la fin de la rétraction)
            emissiveStrenght = Mathf.Lerp(maxEmissiveStrenght, minEmissiveStrenght, t);

            mat.SetFloat("EmissiveStrength_", emissiveStrenght);

            yield return new WaitForSeconds(refreshRate);
        }

        // ****************************************
        // 2. ARRÊTER LES PARTICULES (une fois le max atteint)
        // ****************************************
        if (growing && vinesVFX != null)
        {
            vinesVFX.Stop();
        }

        // Finalisation des valeurs
        mat.SetFloat("Grow_", targetGrow);
        mat.SetFloat("EmissiveStrength_", targetEmissive);

        // L'état est terminé
        isGrowingOrRetracting = false;
    }
}