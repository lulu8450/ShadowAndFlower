using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsManager : MonoBehaviour
{
    [Header("Blocs à afficher")]
    public GameObject[] creditBlocks;

    [Header("Durées (en secondes)")]
    public float fadeInTime = 0.7f;
    public float holdTime = 4f;
    public float fadeOutTime = 0.7f;

    [Header("Gestion du Skip")]
    public float skipAvailableDelay = 5f;
    public Button skipButton;
    public string mainMenuSceneName = "MainMenu";

    [Header("Animation de déplacement")]
    public float slideDistance = 900f;
    public float elasticStrength = 1.15f;

    [Header("Clignotement du bouton Skip")]
    public float skipBlinkSpeed = 1.2f;

    private Coroutine skipBlinkCoroutine;

    private bool skipping = false;

    void Start()
    {
        // Masquer le bouton skip au début
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.onClick.AddListener(SkipCredits);
        }

        // Start after frame
        StartCoroutine(ShowSkipButtonDelayed());
        StartCoroutine(RunCreditsSequence());
    }

    IEnumerator RunCreditsSequence()
    {
        foreach (GameObject block in creditBlocks)
        {
            if (skipping) break;

            yield return StartCoroutine(PlayBlock(block));
        }

        EndCredits();
    }

    IEnumerator PlayBlock(GameObject block)
    {
        CanvasGroup cg = block.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = block.AddComponent<CanvasGroup>();

        RectTransform rt = block.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Credit block sans RectTransform : " + block.name);
            yield break;
        }

        block.SetActive(true);

        float direction = Random.value > 0.5f ? 1f : -1f;

        Vector2 centerPos = rt.anchoredPosition;
        Vector2 startPos = centerPos + Vector2.right * slideDistance * direction;
        Vector2 exitPos = centerPos - Vector2.right * slideDistance * direction;

        rt.anchoredPosition = startPos;
        rt.localScale = Vector3.one * 0.85f;
        cg.alpha = 0f;

        float t = 0f;
        while (t < fadeInTime)
        {
            if (skipping) yield break;

            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeInTime);

            rt.anchoredPosition = Vector2.Lerp(startPos, centerPos, EaseOutCubic(a));
            rt.localScale = Vector3.one * Mathf.Lerp(0.85f, elasticStrength, a);
            cg.alpha = a;

            yield return null;
        
        }

        t = 0f;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.one * Mathf.Lerp(elasticStrength, 1f, t / 0.15f);
            yield return null;
        
        }

        rt.localScale = Vector3.one;
        rt.anchoredPosition = centerPos;
        cg.alpha = 1f;

        float timer = 0f;
        while (timer < holdTime)
        {
            if (skipping) yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < fadeOutTime)
        {
            if (skipping) yield break;

            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeOutTime);

            rt.anchoredPosition = Vector2.Lerp(centerPos, exitPos, EaseInCubic(a));
            cg.alpha = 1f - a;
            rt.localScale = Vector3.one * Mathf.Lerp(1f, 0.9f, a);

            yield return null;

            
        }
        block.SetActive(false);
    }

    IEnumerator ShowSkipButtonDelayed()
    {
        yield return new WaitForSeconds(skipAvailableDelay);
        if (!skipping && skipButton != null)
            {
                skipButton.gameObject.SetActive(true);
                skipBlinkCoroutine = StartCoroutine(BlinkSkipButton());
            }
    }

    IEnumerator BlinkSkipButton()
    { 
        CanvasGroup cg = skipButton.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = skipButton.gameObject.AddComponent<CanvasGroup>();

        while (!skipping)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * skipBlinkSpeed;
                cg.alpha = Mathf.Lerp(1f,0.3f,t);
                yield return null;
            
            }

            t = 0f;
            while (t<1f)
            {
                t+= Time.deltaTime * skipBlinkSpeed;
                cg.alpha = Mathf.Lerp(0.3f,1f,t);
                yield return null;
            }
        }
    }

    public void SkipCredits()
    {
        skipping = true;

        if (skipBlinkCoroutine != null)
            StopCoroutine(skipBlinkCoroutine);

        StopAllCoroutines();
        EndCredits();
    }

    void EndCredits()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    float EaseInCubic(float t)
    { 
        return t * t * t;
    }
}
