using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : StaticInstance<AudioManager> {
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [SerializeField] private ScriptableAudio audioClips;
    public ScriptableAudio AudioClips => audioClips;

    public void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySound(AudioClip audioClip, float vol) {
        SFXSource.PlayOneShot(audioClip, vol);
    }

    public void PlaySound(AudioClips audioClips) {

        if (audioClips.Clips == null || audioClips.Clips.Count() == 0) {
            Debug.LogWarning("Tried playing sound with no audio clips");
            return;
        }

        AudioClip audioClip = audioClips.Clips.RandomItem();
        PlaySound(audioClip, audioClips.Volume);
    }

    private List<AudioClipsTimer> audioClipsTimers = new();

    // when multiple of the same sound are played at the same time, ignore all but the first one
    public void PlaySingleSound(AudioClips audioClips, float ignoreTime = 0.1f) {

        if (audioClipsTimers.Any(x => x.Clips == audioClips)) {
            return;
        }

        PlaySound(audioClips);

        audioClipsTimers.Add(new AudioClipsTimer(audioClips, ignoreTime));
    }

    private void Update() {
        for (int i = 0; i < audioClipsTimers.Count; i++) {
            audioClipsTimers[i].Timer -= Time.deltaTime;

            if (audioClipsTimers[i].Timer < 0f) {
                audioClipsTimers.RemoveAt(i);
                i--;
            }
        }
    }
}

[Serializable]
public struct AudioClips {
    public AudioClip[] Clips;
    [Range(0f, 1f)] public float Volume;

    // Overriding Equals method
    public override bool Equals(object obj) {
        if (!(obj is AudioClips other)) return false;

        // Compare Volume
        if (!Mathf.Approximately(Volume, other.Volume)) return false;

        // Compare Clips array
        if ((Clips == null && other.Clips != null) || (Clips != null && other.Clips == null)) return false;
        if (Clips != null && other.Clips != null && !Clips.SequenceEqual(other.Clips)) return false;

        return true;
    }

    // Overriding GetHashCode
    public override int GetHashCode() {
        int hash = Volume.GetHashCode();
        if (Clips != null) {
            foreach (var clip in Clips) {
                hash = hash * 31 + (clip != null ? clip.GetHashCode() : 0);
            }
        }
        return hash;
    }

    // Equality operator
    public static bool operator ==(AudioClips left, AudioClips right) {
        return left.Equals(right);
    }

    // Inequality operator
    public static bool operator !=(AudioClips left, AudioClips right) {
        return !(left == right);
    }
}

public class AudioClipsTimer {
    public AudioClips Clips;
    public float Timer;

    public AudioClipsTimer(AudioClips clips, float timer) {
        Clips = clips;
        Timer = timer;
    }
}