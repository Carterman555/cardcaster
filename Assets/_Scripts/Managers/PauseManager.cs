using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : StaticInstance<PauseManager> {

    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private MMF_Player pausePlayer;

    private bool paused;

    private void Update() {
        if (pauseAction.action.triggered) {
            PauseGame();
        }
    }

    public void PauseGame() {
        if (!paused && !pausePlayer.IsPlaying) {
            paused = true;
            pausePlayer.PlayFeedbacks();
        }
    }

    // used by resume button and closable panel onclose event
    public void UnpauseGame() {
        if (paused && !pausePlayer.IsPlaying) {
            paused = false;
            pausePlayer.PlayFeedbacks();
        }
    }

    public bool IsPaused() {
        return paused;
    }
}
