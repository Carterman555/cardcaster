using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : StaticInstance<PauseManager> {

    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private InputAction pauseAction;

    private bool paused;

    protected override void Awake() {
        base.Awake();
        pauseAction.Enable();
    }

    private void Update() {
        if (pauseAction.triggered) {
            TogglePause();
        }
    }

    public void TogglePause() {
        if (paused) {
            UnpauseGame();
        }
        else if (!paused) {
            PauseGame();
        }
    }

    public void PauseGame() {
        paused = true;
        Time.timeScale = 0f;

        playerInput.SwitchCurrentActionMap("UI");
    }

    public void UnpauseGame() {
        paused = false;
        Time.timeScale = 1f;

        playerInput.SwitchCurrentActionMap("Gameplay");
    }
}
