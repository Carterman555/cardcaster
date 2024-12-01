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

        if (FeedbackPlayer.GetPlayer("PausePanel").IsPlaying) {
            return;
        }

        paused = true;
        Time.timeScale = 0f;

        FeedbackPlayer.Play("PausePanel");
    }

    public void TryUnpauseGame() {

        if (FeedbackPlayer.GetPlayer("PausePanel").IsPlaying) {
            return;
        }

        paused = false;
        Time.timeScale = 1f;

        FeedbackPlayer.PlayInReverse("PausePanel");
    }
}
