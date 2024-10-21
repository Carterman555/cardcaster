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
    Paused,
    CutScene
}
