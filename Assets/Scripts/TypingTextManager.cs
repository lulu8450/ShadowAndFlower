using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingTextManager : MonoBehaviour
{
    public static TypingTextManager Instance { get; private set; }

    // Pour gérer plusieurs textes en même temps
    Dictionary<TMP_Text, Coroutine> activeCoroutines = new Dictionary<TMP_Text, Coroutine>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(TMP_Text textUI, string fullText, float speed)
    {
        if (textUI == null) return;

        // Stop ancien typing sur ce texte
        if (activeCoroutines.ContainsKey(textUI))
        {
            StopCoroutine(activeCoroutines[textUI]);
            activeCoroutines.Remove(textUI);
        }

        Coroutine c = StartCoroutine(TypeRoutine(textUI, fullText, speed));
        activeCoroutines.Add(textUI, c);
    }

    public void Skip(TMP_Text textUI, string fullText)
    {
        if (textUI == null) return;

        if (activeCoroutines.ContainsKey(textUI))
        {
            StopCoroutine(activeCoroutines[textUI]);
            activeCoroutines.Remove(textUI);
        }

        textUI.text = fullText;
    }

    IEnumerator TypeRoutine(TMP_Text textUI, string fullText, float speed)
    {
        textUI.text = "";

        speed = Mathf.Max(0.01f, speed);
        float delay = 0.05f / speed;

        foreach (char c in fullText)
        {
            textUI.text += c;
            yield return new WaitForSeconds(delay);
        }

        activeCoroutines.Remove(textUI);
    }
}
