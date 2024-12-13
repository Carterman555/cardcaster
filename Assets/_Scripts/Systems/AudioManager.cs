using System;
using UnityEngine;

public class AudioManager : StaticInstance<AudioManager> {
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource TestSource;

    [SerializeField] private ScriptableAudio audioClips;
    public ScriptableAudio AudioClips => audioClips;

    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(AudioClip audioClip, float vol) {
        SFXSource.PlayOneShot(audioClip, vol);
    }

    public void PlayRandomSound(AudioClips audioClips) {
        AudioClip audioClip = audioClips.Clips.RandomItem();
        PlaySound(audioClip, audioClips.Volume);
    }
}

[Serializable]
public struct AudioClips {
    public AudioClip[] Clips;
    [Range(0f, 1f)] public float Volume;
}