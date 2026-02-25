using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f); // Prevent log of zero or negative
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}

