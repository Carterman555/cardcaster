using MoreMountains.Feedbacks;
using QFSW.QC;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager> {

    public static event Action OnLoadingCompleted;

    public static event Action OnStartGameLoadingStarted;
    public static event Action OnStartGameLoadingCompleted;

    public static event Action<int> OnLevelComplete;

    [SerializeField] private bool debugStartTutorial;

    public bool InTutorial { get; private set; }

    public EnvironmentType CurrentEnvironment { get; private set; }
    public int Level { get; private set; }

    [SerializeField] private int debugStartingLevel = 1;

    private MMF_Player sceneLoadPlayer;

    public bool IsSceneLoading { get; private set; }

    // needed only for when starting game in game scene
    protected override void Awake() {
        
        // only play this setup code for first instance
        if (Instance != null) {
            base.Awake(); // this will destroy the new instance
            return;
        }

        //... this set the instance to this
        base.Awake();

        Level = 1;
        Level = debugStartingLevel;

        UpdateEnvironmentType();
        InTutorial = debugStartTutorial;

        sceneLoadPlayer = GetComponent<MMF_Player>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game")) {
            OnStartGameLoadingStarted?.Invoke();
        }
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void StartGame(bool tutorial = false) {
        Level = 1;
        Level = debugStartingLevel;

        UpdateEnvironmentType();
        InTutorial = tutorial;

        LoadGameScene();

        OnStartGameLoadingStarted?.Invoke();
    }

    [Command]
    public void NextLevel() {
        OnLevelComplete?.Invoke(Level);

        Level++;

        UpdateEnvironmentType();
        LoadGameScene();
    }

    public void LoadTutorial() {
        InTutorial = true;
        LoadGameScene();
    }

    public void LoadMenu() {
        LoadMenuScene();
    }

    private void LoadGameScene() {
        MMF_LoadScene loadGameFeedback = sceneLoadPlayer.GetFeedbackOfType<MMF_LoadScene>("Load Game Scene");
        loadGameFeedback.Play(Vector3.zero);

        IsSceneLoading = true;
    }

    private void LoadMenuScene() {
        MMF_LoadScene loadGameFeedback = sceneLoadPlayer.GetFeedbackOfType<MMF_LoadScene>("Load Menu Scene");
        loadGameFeedback.Play(Vector3.zero);

        IsSceneLoading = true;
    }

    private void UpdateEnvironmentType() {
        // with levelsPerEnvironment = 2 then level 1 and 2 become 0 (stone), 3 and 4 become 1 (smooth stone), and 5 and 6 become 2 (blue stone)
        int uniqueEnvironments = Enum.GetValues(typeof(EnvironmentType)).Length;
        int environmentNum = (Level - 1) % uniqueEnvironments;
        CurrentEnvironment = (EnvironmentType)environmentNum;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        if (scene != SceneManager.GetSceneByName("AdditiveLoadingScreen")) {
            //if (Level == 1) {
            if (Level == debugStartingLevel) {
                OnStartGameLoadingCompleted?.Invoke();
            }
        }
    }

    private void OnSceneUnloaded(Scene scene) {
        if (scene == SceneManager.GetSceneByName("AdditiveLoadingScreen")) {
            IsSceneLoading = false;
            OnLoadingCompleted?.Invoke();
        }
        else if (scene == SceneManager.GetSceneByName("Game")) {
            ObjectPoolManager.ResetPools();
        }
    }

    public int GetEnvironmentLevel() {
        float uniqueEnvironments = Enum.GetValues(typeof(EnvironmentType)).Length;
        return Mathf.CeilToInt(Level / uniqueEnvironments);
    }


    [Command]
    private void SkipLevels(int amount) {
        OnLevelComplete?.Invoke(Level);

        Level += amount;

        UpdateEnvironmentType();
        LoadGameScene();
    }
}
