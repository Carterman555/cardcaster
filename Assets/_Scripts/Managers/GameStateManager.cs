using System;

public class GameStateManager : StaticInstance<GameStateManager> {

    public static event Action<GameState> OnGameStateChanged;

    private static bool inDemo = true;
    public static bool InDemo => inDemo;

    private GameState currentState;

    protected override void Awake() {
        base.Awake();

        //... the game is loading until the room is generated
        currentState = GameState.Loading;
    }

    private void OnEnable() {
        GameSceneManager.OnLoadingCompleted += OnGameStart;
        InputManager.OnActionMapChanged += OnActionMapChanged;

        if (!GameSceneManager.Instance.IsSceneLoading) {
            OnGameStart();
        }
    }

    private void OnDisable() {
        GameSceneManager.OnLoadingCompleted -= OnGameStart;
        InputManager.OnActionMapChanged -= OnActionMapChanged;
    }

    private void OnGameStart() {
        SetGameState(GameState.Game);
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
    UI,
    Loading
}
