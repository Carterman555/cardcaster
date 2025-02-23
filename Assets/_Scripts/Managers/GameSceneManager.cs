using Mono.CSharp;
using MoreMountains.Feedbacks;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager> {

    public static event Action OnStartGame;

    [SerializeField] private bool debugStartTutorial;

    public bool Tutorial { get; private set; }

    private EnvironmentType currentEnvironment;
    private int level;

    private const int LEVELS_PER_ENVIRONMENT = 1;

    private MMF_Player sceneLoadPlayer;

    // needed only for when starting game in game scene
    protected override void Awake() {
        base.Awake();

        level = 1;
        currentEnvironment = EnvironmentType.Stone;
        Tutorial = debugStartTutorial;

        sceneLoadPlayer = GetComponent<MMF_Player>();

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game")) {
            OnStartGame?.Invoke();
        }
    }

    public void StartGame(bool tutorial = false) {
        level = 1;
        currentEnvironment = EnvironmentType.Stone;
        Tutorial = tutorial;

        LoadGameScene();

        OnStartGame?.Invoke();
    }

    [Command]
    public void NextLevel() {
        level++;
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
        MMF_LoadScene loadSceneFeedback = sceneLoadPlayer.GetFeedbackOfType<MMF_LoadScene>();
        loadSceneFeedback.DestinationSceneName = "Game";
        loadSceneFeedback.WaitForMethodCallToUnload = true; // wait for room generation to complete before transitioning to game scene

        sceneLoadPlayer.PlayFeedbacks();
    }

    private void LoadMenuScene() {
        MMF_LoadScene loadSceneFeedback = sceneLoadPlayer.GetFeedbackOfType<MMF_LoadScene>();
        loadSceneFeedback.DestinationSceneName = "Menu";
        loadSceneFeedback.WaitForMethodCallToUnload = false;

        sceneLoadPlayer.PlayFeedbacks();
    }

    private void UpdateEnvironmentType() {

        // with levelsPerEnvironment = 2 then level 1 and 2 become 0 (stone), 3 and 4 become 1 (smooth stone), and 5 and 6 become 2 (blue stone)
        int environmentNum = Mathf.CeilToInt(((float)level / LEVELS_PER_ENVIRONMENT) - 1);
        currentEnvironment = (EnvironmentType)environmentNum;
    }

    public int GetLevel() {
        return level;
    }

    // the level in the environment
    public int GetSubLevel() {
        int subLevel = ((level - 1) % LEVELS_PER_ENVIRONMENT) + 1;
        return subLevel;
    }

    public EnvironmentType GetEnvironment() {
        return currentEnvironment;
    }
}
