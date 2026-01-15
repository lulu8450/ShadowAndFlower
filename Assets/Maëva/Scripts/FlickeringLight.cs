using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Réglages de l'Intensité")]
    [Tooltip("L'intensité minimale de la lumière")]
    public float minIntensity = 2f;
    [Tooltip("L'intensité maximale de la lumière")]
    public float maxIntensity = 4f;
    [Tooltip("La vitesse du scintillement. Plus c'est haut, plus c'est nerveux.")]
    public float flickerSpeed = 10f;

    [Header("Réglages du Mouvement")]
    [Tooltip("La lumière doit-elle bouger légèrement ?")]
    public bool enableMovement = true;
    [Tooltip("La distance maximale de mouvement autour de la position initiale")]
    public float movementAmount = 0.1f;
    [Tooltip("La vitesse du mouvement")]
    public float movementSpeed = 5f;

    private Light fireLight;
    private float randomOffset;
    private Vector3 initialPosition;

    void Start()
    {
        // On récupère le composant Light sur cet objet
        fireLight = GetComponent<Light>();
        if (fireLight == null)
        {
            Debug.LogError("Ce script doit être attaché à un objet avec un composant Light !");
            enabled = false;
            return;
        }

        // On mémorise la position de départ pour tourner autour
        initialPosition = transform.localPosition;
        // Un décalage aléatoire pour que toutes les flammes de la scène ne clignotent pas pareil
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 1. Gérer le scintillement (Intensité)
        // On utilise Mathf.PerlinNoise pour un changement fluide et naturel, pas juste aléatoire bruyant.
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f);
        // On transforme le résultat du noise (entre 0 et 1) en une valeur entre min et max intensity
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // 2. Gérer le micro-mouvement (Position)
        if (enableMovement)
        {
            // On génère 3 bruits différents pour X, Y et Z
            float x = (Mathf.PerlinNoise(Time.time * movementSpeed + randomOffset, 1f) - 0.5f) * 2f * movementAmount;
            float y = (Mathf.PerlinNoise(Time.time * movementSpeed + randomOffset, 2f) - 0.5f) * 2f * movementAmount;
            float z = (Mathf.PerlinNoise(Time.time * movementSpeed + randomOffset, 3f) - 0.5f) * 2f * movementAmount;

            // On applique la nouvelle position par rapport à la position initiale
            transform.localPosition = initialPosition + new Vector3(x, y, z);
        }
    }
}