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

        Transform tr = block.transform;

        block.SetActive(true);
        cg.alpha = 0f;
        tr.localScale = Vector3.one * 0.95f;

        // --- FADE IN ---
        float t = 0f;
        while (t < fadeInTime)
        {
            if (skipping) yield break;

            t += Time.deltaTime;
            float a = t / fadeInTime;

            cg.alpha = a;
            tr.localScale = Vector3.one * Mathf.Lerp(0.95f, 1f, a);

            yield return null;
        }

        // --- HOLD ---
        float timer = 0f;
        while (timer < holdTime)
        {
            if (skipping) yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        // --- FADE OUT ---
        t = 0f;
        while (t < fadeOutTime)
        {
            if (skipping) yield break;

            t += Time.deltaTime;
            float a = t / fadeOutTime;

            cg.alpha = 1f - a;
            tr.localScale = Vector3.one * Mathf.Lerp(1f, 1.05f, a);

            yield return null;
        }

        block.SetActive(false);
    }

    IEnumerator ShowSkipButtonDelayed()
    {
        yield return new WaitForSeconds(skipAvailableDelay);
        if (!skipping && skipButton != null)
            skipButton.gameObject.SetActive(true);
    }

    public void SkipCredits()
    {
        skipping = true;
        StopAllCoroutines();
        EndCredits();
    }

    void EndCredits()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
