using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxOnEvents : MonoBehaviour {

    [SerializeField] private bool positional;
    [SerializeField] private bool singleSound;

    [SerializeField] private bool hasSfxOnEnable;
    [SerializeField, ConditionalHide("hasSfxOnEnable")] private AudioClips enableSfx;

    [SerializeField] private bool hasSfxOnDisable;
    [SerializeField, ConditionalHide("hasSfxOnDisable")] private AudioClips disableSfx;

    private void OnEnable() {
        if (hasSfxOnEnable) {
            PlaySound(enableSfx);
        }
    }

    private void OnDisable() {

        if (Helpers.GameStopping()) {
            return;
        }

        if (hasSfxOnDisable) {
            PlaySound(disableSfx);
        }
    }

    private void PlaySound(AudioClips sfx) {
        Vector2 position = positional ? transform.position : Vector2.zero;
        if (singleSound) {
            AudioManager.Instance.PlaySingleSound(sfx, position: position);
        }
        else {
            AudioManager.Instance.PlaySound(sfx, position: position);
        }
    }
}
