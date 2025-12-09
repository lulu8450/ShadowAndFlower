using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;
    [Header("Mixer")]
    public AudioMixer masterMixer;

    [Header("Music")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    private AudioSource activeMusic, idleMusic;
    public float musicFadeTime = 1f;

    [Header("SFX Pool")]
    public GameObject sfxSourcePrefab;
    public int sfxPoolSize = 12;
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    [Header("Voice")]
    public AudioSource voiceSource;

    void Awake()
    {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        activeMusic = musicSourceA;
        idleMusic = musicSourceB;

        // init pool
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject go = Instantiate(sfxSourcePrefab, transform);
            AudioSource a = go.GetComponent<AudioSource>();
            a.playOnAwake = false;
            sfxPool.Enqueue(a);
        }
    }

    // ----- Volumes (0..1) -----
    public void SetMasterVolume(float v) => masterMixer.SetFloat("volume_master", Mathf.Log10(Mathf.Max(v,0.0001f)) * 20f);
    public void SetMusicVolume(float v) => masterMixer.SetFloat("volume_music", Mathf.Log10(Mathf.Max(v,0.0001f)) * 20f);
    public void SetSFXVolume(float v) => masterMixer.SetFloat("volume_sfx", Mathf.Log10(Mathf.Max(v,0.0001f)) * 20f);
    public void SetVoiceVolume(float v) => masterMixer.SetFloat("volume_voice", Mathf.Log10(Mathf.Max(v,0.0001f)) * 20f);
    public void SetAmbienceVolume(float v) => masterMixer.SetFloat("volume_ambience", Mathf.Log10(Mathf.Max(v,0.0001f)) * 20f);

    // ----- Music crossfade -----
    public void PlayMusic(AudioClip clip, float fadeTime = -1f)
    {
        if (fadeTime < 0) fadeTime = musicFadeTime;
        StartCoroutine(CrossfadeMusic(clip, fadeTime));
    }

    IEnumerator CrossfadeMusic(AudioClip newClip, float t)
    {
        idleMusic.clip = newClip;
        idleMusic.volume = 0f;
        idleMusic.Play();

        float elapsed = 0f;
        while (elapsed < t)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = elapsed / t;
            activeMusic.volume = 1f - p;
            idleMusic.volume = p;
            yield return null;
        }

        activeMusic.Stop();
        // swap
        var tmp = activeMusic;
        activeMusic = idleMusic;
        idleMusic = tmp;
    }

    // ----- SFX play (pooled) -----
    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        AudioSource src;
        if (sfxPool.Count > 0) src = sfxPool.Dequeue();
        else { var go = Instantiate(sfxSourcePrefab, transform); src = go.GetComponent<AudioSource>(); }

        src.transform.position = position;
        src.spatialBlend = 1f; // 3D
        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.Play();
        StartCoroutine(ReturnToPoolWhenDone(src));
    }

    IEnumerator ReturnToPoolWhenDone(AudioSource src)
    {
        yield return new WaitUntil(() => !src.isPlaying);
        sfxPool.Enqueue(src);
    }

    // ----- Voice play (2D) -----
    public void PlayVoice(AudioClip clip)
    {
        if (clip == null) return;
        voiceSource.clip = clip;
        voiceSource.spatialBlend = 0f;
        voiceSource.Play();
        // optional: duck music while voice plays
        StopAllCoroutines();
        StartCoroutine(DuckMusicDuringVoice());
    }

    IEnumerator DuckMusicDuringVoice()
    {
        // fade music down
        float t = 0.25f;
        float start = activeMusic.volume;
        float target = 0.3f;
        float elapsed = 0f;
        while (elapsed < t)
        {
            elapsed += Time.unscaledDeltaTime;
            activeMusic.volume = Mathf.Lerp(start, target, elapsed / t);
            yield return null;
        }

        // wait voice done
        yield return new WaitUntil(() => !voiceSource.isPlaying);

        // fade back up
        elapsed = 0f;
        while (elapsed < t)
        {
            elapsed += Time.unscaledDeltaTime;
            activeMusic.volume = Mathf.Lerp(target, 1f, elapsed / t);
            yield return null;
        }
    }
}
