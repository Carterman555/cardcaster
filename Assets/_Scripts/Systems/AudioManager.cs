using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager> {
    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    [SerializeField] private ScriptableAudio audioClips;
    public ScriptableAudio AudioClips => audioClips;

    private List<AudioClipsTimer> audioClipsTimers = new();

    private void Update() {
        for (int i = 0; i < audioClipsTimers.Count; i++) {
            audioClipsTimers[i].Timer -= Time.deltaTime;

            if (audioClipsTimers[i].Timer < 0f) {
                audioClipsTimers.RemoveAt(i);
                i--;
            }
        }

        HandleSFXFadeFromUI();
    }

    

    public GameObject PlaySound(AudioClips audioClips, bool uiSound = false, bool loop = false) {

        if (audioClips.Clips == null || audioClips.Clips.Count() == 0) {
            Debug.LogWarning("Tried playing sound with no audio clips");
            return null;
        }

        AudioClip audioClip = audioClips.Clips.RandomItem();
        AudioMixerGroup audioMixerGroup = uiSound ? uiMixerGroup : sfxMixerGroup;
        float pitch = 1f + UnityEngine.Random.Range(-audioClips.PitchVariation, audioClips.PitchVariation);

        return PlaySound(audioClip, audioMixerGroup, audioClips.Volume, pitch, loop);
    }

    // when multiple of the same sound are played at the same time, ignore all but the first one
    public GameObject PlaySingleSound(AudioClips audioClips, float ignoreTime = 0.1f, bool uiSound = false) {

        if (audioClipsTimers.Any(x => x.Clips == audioClips)) {
            return null;
        }

        GameObject audioSourceGO = PlaySound(audioClips, uiSound);

        audioClipsTimers.Add(new AudioClipsTimer(audioClips, ignoreTime));

        return audioSourceGO;
    }

    public GameObject PlaySound(AudioClip audioClip, AudioMixerGroup audioMixerGroup, float vol, float pitch = 1f, bool loop = false) {
        GameObject audioSourceGO = audioSourcePrefab.Spawn(transform);
        AudioSource audioSource = audioSourceGO.GetComponent<AudioSource>();

        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.volume = vol;
        audioSource.pitch = pitch;
        audioSource.loop = loop;

        audioSource.Play();

        if (!loop) {
            StartCoroutine(ReturnOnComplete(audioSourceGO, audioClip.length));
        }

        return audioSourceGO;
    }

    private IEnumerator ReturnOnComplete(GameObject audioSourceGO, float clipLength) {
        yield return new WaitForSeconds(clipLength);

        audioSourceGO.TryReturnToPool();
    }

    #region Stop SFX When In UI

    private Tween volumeFade;
    private float sfxVolume;

    private void OnEnable() {
        InputManager.OnActionMapChanged += OnActionMapChanged;
    }

    private void OnDisable() {
        InputManager.OnActionMapChanged -= OnActionMapChanged;
    }

    // if the controls changed to gameplay, fade the sfx volume to the value set in settings. If the controls change to UI, fade the sfx
    // volume to 0. There shouldn't be sfx playing when ui is open
    private void OnActionMapChanged(string actionMap) {
        if (actionMap == "Gameplay") {
            sfxMixerGroup.audioMixer.GetFloat("SfxVolume", out sfxVolume);

            float targetVolume = SliderValueToDecibels(SettingsManager.CurrentSettings.SFXVolume);

            volumeFade = DOTween.To(() => sfxVolume, v => sfxVolume = v, targetVolume, duration: 0.5f).SetUpdate(true);
        }
        else if (actionMap == "UI") {
            sfxMixerGroup.audioMixer.GetFloat("SfxVolume", out sfxVolume);
            volumeFade = DOTween.To(() => sfxVolume, v => sfxVolume = v, -80f, duration: 0.5f).SetUpdate(true);
        }
    }

    private void HandleSFXFadeFromUI() {
        if (volumeFade != null && volumeFade.IsActive() && volumeFade.IsPlaying()) {
            sfxMixerGroup.audioMixer.SetFloat("SfxVolume", sfxVolume);
        }
    }

    #endregion

    public static float SliderValueToDecibels(float sliderValue, float maxDB = 0f) {
        //... Clamp and prepare slider input
        sliderValue = Mathf.Clamp01(sliderValue);

        // Calculate amplitude range
        float minAmplitude = 0.0001f; // -80dB
        float maxAmplitude = Mathf.Pow(10f, maxDB / 20f); // Convert maxDB to amplitude

        //... Map slider to amplitude range
        float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, sliderValue);

        //... Convert to dB
        return Mathf.Log10(amplitude) * 20f;
    }
}

[Serializable]
public struct AudioClips {
    public AudioClip[] Clips;
    [Range(0f, 1f)] public float Volume;
    public float PitchVariation;

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