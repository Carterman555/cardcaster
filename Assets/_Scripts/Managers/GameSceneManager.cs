using Mono.CSharp;
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

    private const int LEVELS_PER_ENVIRONMENT = 2;

    // needed only for when starting game in game scene
    protected override void Awake() {
        base.Awake();

        level = 1;
        currentEnvironment = EnvironmentType.Stone;
        Tutorial = debugStartTutorial;
    }

    public void StartGame(bool tutorial = false) {
        level = 1;
        currentEnvironment = EnvironmentType.Stone;
        Tutorial = tutorial;

        StartCoroutine(LoadGameScene());

        OnStartGame?.Invoke();
    }

    [Command]
    public void NextLevel() {
        level++;
        UpdateEnvironmentType();
        StartCoroutine(LoadGameScene());
    }

    public void LoadTutorial() {
        Tutorial = true;
        StartCoroutine(LoadGameScene());
    }

    public void LoadMenu() {
        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadGameScene() {
        yield return StartCoroutine(SceneTransitionManager.Instance.PlayStartTransition());
        SceneManager.LoadScene("Game");

        SceneTransitionManager.Instance.PlayEndTransition();
    }

    private IEnumerator LoadMenuScene() {
        yield return StartCoroutine(SceneTransitionManager.Instance.PlayStartTransition());
        SceneManager.LoadScene("Menu");

        SceneTransitionManager.Instance.PlayEndTransition();
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
