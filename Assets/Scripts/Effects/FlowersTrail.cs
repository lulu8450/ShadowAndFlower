using UnityEngine;
using System.Collections.Generic;

public class FlowersTrail : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] float fadeInDuration = 0.5f;
    [SerializeField] float lifeDuration = 1.0f;
    [SerializeField] float fadeOutDuration = 1.5f;

    // NOUVELLE LISTE DE PROPRIÉTÉS À CIBLER
    private static readonly string[] ColorProperties = { "_FlowerColor1", "_FlowerColor2" };

    private List<Renderer> flowerRenderers;
    private List<List<Color>> startColorsPerRenderer; // Liste de listes pour stocker les couleurs initiales

    float timer;

    enum FadeState { FadeIn, Life, FadeOut, Done }
    FadeState currentState = FadeState.FadeIn;

    void Awake()
    {
        flowerRenderers = new List<Renderer>();
        startColorsPerRenderer = new List<List<Color>>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
        {
            Debug.LogError("FlowerTrail: Aucun Renderer trouvé.");
            Destroy(gameObject);
            return;
        }

        foreach (Renderer r in renderers)
        {
            if (r.material != null)
            {
                // Liste temporaire pour les couleurs initiales de CE Renderer
                List<Color> currentRendererColors = new List<Color>();
                bool foundAnyColor = false;

                // On vérifie chaque propriété de couleur
                foreach (string propName in ColorProperties)
                {
                    if (r.material.HasProperty(propName))
                    {
                        currentRendererColors.Add(r.material.GetColor(propName));
                        foundAnyColor = true;
                    }
                }

                if (foundAnyColor)
                {
                    // Si au moins une couleur a été trouvée, on ajoute le Renderer et ses couleurs
                    flowerRenderers.Add(r);
                    startColorsPerRenderer.Add(currentRendererColors);
                }
                else
                {
                    Debug.LogWarning($"Le matériel sur {r.gameObject.name} n'a aucune des propriétés de couleur ciblées.");
                }
            }
        }

        if (flowerRenderers.Count > 0)
        {
            SetAlpha(0f);
        }
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case FadeState.FadeIn:
                float fadeInT = Mathf.Clamp01(timer / fadeInDuration);
                SetAlpha(fadeInT);
                if (timer >= fadeInDuration)
                {
                    currentState = FadeState.Life;
                    timer = 0f;
                }
                break;

            case FadeState.Life:
                if (timer >= lifeDuration)
                {
                    currentState = FadeState.FadeOut;
                    timer = 0f;
                }
                break;

            case FadeState.FadeOut:
                float fadeOutT = Mathf.Clamp01(timer / fadeOutDuration);
                SetAlpha(1f - fadeOutT);
                if (timer >= fadeOutDuration)
                {
                    currentState = FadeState.Done;
                    Destroy(gameObject);
                }
                break;

            case FadeState.Done:
                break;
        }
    }

    void SetAlpha(float alpha)
    {
        for (int i = 0; i < flowerRenderers.Count; i++)
        {
            Renderer r = flowerRenderers[i];
            List<Color> baseColors = startColorsPerRenderer[i]; // Récupère les couleurs originales

            // Si c'est un SpriteRenderer (méthode standard)
            if (r is SpriteRenderer sr)
            {
                Color newColor = sr.color;
                newColor.a = alpha;
                sr.color = newColor;
            }
            // Si c'est un MeshRenderer ou autre Renderer 3D
            else if (r.material != null)
            {
                // On boucle sur toutes les couleurs initiales stockées pour ce Renderer
                for (int j = 0; j < baseColors.Count; j++)
                {
                    // L'ordre des couleurs dans baseColors correspond à l'ordre dans ColorProperties
                    string propName = ColorProperties[j];
                    Color baseColor = baseColors[j];

                    Color newColor = baseColor;
                    newColor.a = alpha;

                    // On applique la nouvelle couleur (avec l'alpha modifié) à la propriété spécifique
                    r.material.SetColor(propName, newColor);
                }
            }
        }
    }
}