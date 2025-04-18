using MoreMountains.Feedbacks;
using QFSW.QC;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager> {

    public static event Action OnStartGameLoadingStarted;
    public static event Action OnStartGameLoadingCompleted;

    public static event Action<int> OnLevelComplete;
    public static event Action OnWinGame;

    [SerializeField] private bool debugStartTutorial;

    public bool Tutorial { get; private set; }

    public EnvironmentType CurrentEnvironment { get; private set; }
    public int Level { get; private set; }

    private const int LEVELS_PER_ENVIRONMENT = 1;

    private const int MAX_LEVEL = 3;

    private MMF_Player sceneLoadPlayer;

    public bool IsSceneLoading { get; private set; }

    // needed only for when starting game in game scene
    protected override void Awake() {
        base.Awake();

        Level = 1;
        CurrentEnvironment = EnvironmentType.Stone;
        Tutorial = debugStartTutorial;

        sceneLoadPlayer = GetComponent<MMF_Player>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChanged;

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game")) {
            OnStartGameLoadingStarted?.Invoke();
        }
    }

    

    public void StartGame(bool tutorial = false) {
        Level = 1;
        CurrentEnvironment = EnvironmentType.Stone;
        Tutorial = tutorial;

        LoadGameScene();

        OnStartGameLoadingStarted?.Invoke();
    }

    [Command]
    public void NextLevel() {
        OnLevelComplete?.Invoke(Level);

        Level++;

        if (Level > MAX_LEVEL) {
            OnWinGame?.Invoke();
            FeedbackPlayerReference.Play("DemoComplete");
            return;
        }

        UpdateEnvironmentType();
        LoadGameScene();
    }

    public void LoadTutorial() {
        Tutorial = true;
        LoadGameScene();
    }

    public void LoadMenu() {
        LoadMenuScene();
    }

    private void LoadGameScene() {
        MMF_LoadScene loadGameFeedback = sceneLoadPlayer.GetFeedbackOfType<MMF_LoadScene>("Load Game Scene");
        loadGameFeedback.Play(Vector3.zero);
        print("load game");

        IsSceneLoading = true;
    }

    private void LoadMenuScene() {
        MMF_LoadScene loadGameFeedback = sceneLoadPlayer.GetFeedbackOfType<MMF_LoadScene>("Load Menu Scene");
        loadGameFeedback.Play(Vector3.zero);
        print("load menu");

        IsSceneLoading = true;
    }

    private void UpdateEnvironmentType() {

        // with levelsPerEnvironment = 2 then level 1 and 2 become 0 (stone), 3 and 4 become 1 (smooth stone), and 5 and 6 become 2 (blue stone)
        int environmentNum = Mathf.CeilToInt(((float)Level / LEVELS_PER_ENVIRONMENT) - 1);
        CurrentEnvironment = (EnvironmentType)environmentNum;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        if (scene != SceneManager.GetSceneByName("AdditiveLoadingScreen")) {
            IsSceneLoading = false;

            print("On Scene Loaded: " + scene.name);

            if (Level == 1) {
                OnStartGameLoadingCompleted?.Invoke();
            }
        }
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene) {
        print("On Scene Changed: previous = " + previousScene.name + ", new = " + newScene.name);
    }

    // the level in the environment
    public int GetSubLevel() {
        int subLevel = ((Level - 1) % LEVELS_PER_ENVIRONMENT) + 1;
        return subLevel;
    }
}
