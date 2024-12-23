using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : StaticInstance<PauseManager> {

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private InputActionReference pauseAction;

    private bool paused;

    private void Update() {
        if (pauseAction.action.triggered) {
            TryPauseGame();
        }
    }

    public void TogglePause() {
        if (paused) {
            TryUnpauseGame();
        }
        else if (!paused) {
            TryPauseGame();
        }
    }

    public void TryPauseGame() {

        if (FeedbackPlayerOld.GetPlayer("PausePanel").IsPlaying) {
            return;
        }

        paused = true;
        Time.timeScale = 0f;

        FeedbackPlayerOld.Play("PausePanel");

    }

    public void TryUnpauseGame() {

        if (FeedbackPlayerOld.GetPlayer("PausePanel").IsPlaying) {
            return;
        }

        paused = false;
        Time.timeScale = 1f;

        FeedbackPlayerOld.PlayInReverse("PausePanel");

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ClosePanel);
    }
}
