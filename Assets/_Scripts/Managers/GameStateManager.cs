using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : StaticInstance<GameStateManager> {

    public static event Action<GameState> OnGameStateChanged;

    private GameState currentState;

    protected override void Awake() {
        base.Awake();

        currentState = GameState.Game;
    }

    private void OnEnable() {
        InputManager.OnActionMapChanged += OnActionMapChanged;
    }
    private void OnDisable() {
        InputManager.OnActionMapChanged -= OnActionMapChanged;
    }

    private void OnActionMapChanged(string mapActionName) {
        if (mapActionName == "UI") {
            SetGameState(GameState.UI);
        }
        else if (mapActionName == "Gameplay") {
            SetGameState(GameState.Game);
        }
    }

    public GameState GetCurrentState() {
        return currentState;
    }

    public void SetGameState(GameState newState) {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState {
    Game,
    CutScene,
    UI
}
