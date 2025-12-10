using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("volume_master", Mathf.Log10(value) * 20f);
    }

    public void SetVoiceVolume(float value)
    {
        mixer.SetFloat("volume_voice", Mathf.Log10(value) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("volume_music", Mathf.Log10(value) * 20f);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("volume_sfx", Mathf.Log10(value) * 20f);
    }
}
