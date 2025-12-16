using UnityEngine;

public class VisualEffectTrigger : MonoBehaviour
{
    [Header("Effet à activer")]
    public GameObject visualEffect;

    [Header("Durée avant désactivation (0 = jamais)")]
    public float disableAfter = 0f;

    public void PlayEffect()
    {
        if (visualEffect == null)
        {
            Debug.LogWarning("Aucun effet visuel assigné !");
            return;
        }

        visualEffect.SetActive(true);

        if (disableAfter > 0f)
        {
            CancelInvoke();
            Invoke(nameof(StopEffect), disableAfter);
        }
    }

    public void StopEffect()
    {
        if (visualEffect != null)
        {
            visualEffect.SetActive(false);
        }
    }
}
