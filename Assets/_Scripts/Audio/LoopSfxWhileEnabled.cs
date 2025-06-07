using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSfxWhileEnabled : MonoBehaviour {

    [SerializeField] private AudioClips sfx;
    private GameObject audioSource;

    private void OnEnable() {
        audioSource = AudioManager.Instance.PlaySound(sfx, loop: true);
    }

    private void OnDisable() {
        if (audioSource != null) {
            audioSource.ReturnToPool();
        }
    }
}
