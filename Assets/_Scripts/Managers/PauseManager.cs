using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : StaticInstance<PauseManager> {

    [SerializeField] private InputActionReference pauseAction;

    private void Update() {
        if (pauseAction.action.triggered) {
            TryPauseGame();
        }
    }

    public void TryPauseGame() {

        if (FeedbackPlayerOld.GetPlayer("PausePanel").IsPlaying) {
            return;
        }

        FeedbackPlayerOld.Play("PausePanel");
    }

    // used by resume button and closable panel onclose event
    public void TryUnpauseGame() {

        if (FeedbackPlayerOld.GetPlayer("PausePanel").IsPlaying) {
            return;
        }

        FeedbackPlayerOld.PlayInReverse("PausePanel");

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ClosePanel);
    }
}
