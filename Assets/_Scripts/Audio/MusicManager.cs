using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : Singleton<MusicManager> {

    [SerializeField] private ScriptableAudio audioClips;
    [SerializeField] private List<MusicSource> musicSources;

    private MusicType activeMusicType = MusicType.None;

    private float musicFadeDuration = 2f;

    private float casualMusicTimer;

    #region Get Methods

    private AudioSource GetActiveMusicSource() {
        return musicSources.FirstOrDefault(s => s.MusicType == activeMusicType).AudioSource;
    }

    private AudioClips GetActiveMusicClips() {
        return audioClips.Music.FirstOrDefault(c => c.MusicType == activeMusicType).AudioClips;
    }

    #endregion

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TryTransitionToCombatMusic;

        BossManager.OnStartBossFight += TransitionToBossMusic;
        BossManager.OnBossKilled += TransitionToCasualMusic;
    }
    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryTransitionToCombatMusic;

        BossManager.OnStartBossFight -= TransitionToBossMusic;
        BossManager.OnBossKilled -= TransitionToCasualMusic;
    }

    private IEnumerator Start() {
        TransitionMusic(MusicType.Casual);

        yield return null;

        TransitionMusic(MusicType.Combat);

        yield return null;

        TransitionMusic(MusicType.Casual);
    }

    private void TryTransitionToCombatMusic(Room room) {
        if (!room.IsRoomCleared) {
            TransitionMusic(MusicType.Combat);
        }
    }

    private void TransitionToBossMusic() {
        TransitionMusic(MusicType.Boss);
    }

    // played by player death feedback
    public void TransitionToCasualMusic() {
        TransitionMusic(MusicType.Casual);
    }

    private void Update() {
        HandleMusicLooping();
        HandleCasualTransition();
    }

    private void HandleMusicLooping() {
        if (!GetActiveMusicSource().isPlaying) {
            PlayRandomActiveSong();
        }
    }

    private void HandleCasualTransition() {
        if (activeMusicType != MusicType.Combat) {
            casualMusicTimer = 0;
            return;
        }

        bool playerInRoom = Room.GetCurrentRoom() != null;
        bool inCombat = playerInRoom && !Room.GetCurrentRoom().IsRoomCleared;
        if (inCombat) {
            casualMusicTimer = 0;
            return;
        }

        float timeToTransitionCasual = 20f;
        casualMusicTimer += Time.deltaTime;
        if (casualMusicTimer > timeToTransitionCasual) {
            casualMusicTimer = 0;
            TransitionMusic(MusicType.Casual);
        }
    }

    private void TransitionMusic(MusicType newMusicType) {

        print("TransitionMusic: " + newMusicType.ToPrettyString());

        MusicType oldMusicType = activeMusicType;
        activeMusicType = newMusicType;

        // don't change song if tried transitioning to music type that is already playing
        if (oldMusicType == newMusicType) {
            return;
        }

        // fade out old music
        if (oldMusicType != MusicType.None) {
            AudioSource oldAudioSource = musicSources.First(s => s.MusicType == oldMusicType).AudioSource;
            oldAudioSource.DOKill();
            oldAudioSource.DOFade(0f, duration: musicFadeDuration).SetUpdate(true).OnComplete(() => {
                oldAudioSource.Stop();
            });
        }

        // fade in and play new music
        PlayRandomActiveSong();
    }

    private void PlayRandomActiveSong() {

        // fade in new music
        GetActiveMusicSource().DOKill();
        GetActiveMusicSource().DOFade(1f, duration: musicFadeDuration).SetUpdate(true);

        //... set the song on the new audio source
        PlayMusic(GetActiveMusicClips());
    }

    private void PlayMusic(AudioClips audioClips) {
        if (audioClips.Clips == null || audioClips.Clips.Count() == 0) {
            Debug.LogWarning("Tried playing music with no audio clips");
            return;
        }

        AudioClip audioClip = audioClips.Clips.RandomItem();
        PlayMusic(audioClip, audioClips.Volume);
    }

    private void PlayMusic(AudioClip audioClip, float vol) {
        GetActiveMusicSource().Stop();
        GetActiveMusicSource().PlayOneShot(audioClip, vol);
    }
}

public enum MusicType { Casual, Combat, Boss, None }

[Serializable]
public struct MusicSource {
    public MusicType MusicType;
    public AudioSource AudioSource;
}

[Serializable]
public struct MusicClips {
    public MusicType MusicType;
    public AudioClips AudioClips;
}