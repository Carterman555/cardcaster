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

    private int level;

    // needed only for when starting game in game scene
    protected override void Awake() {
        base.Awake();

        level = 1;
        Tutorial = debugStartTutorial;
    }

    public void StartGame(bool tutorial = false) {
        level = 1;
        Tutorial = tutorial;

        StartCoroutine(LoadGameScene());

        OnStartGame?.Invoke();
    }

    public void NextLevel() {
        level++;
        StartCoroutine(LoadGameScene());
    }

    public void LoadTutorial() {
        Tutorial = true;
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene() {
        yield return StartCoroutine(SceneTransitionManager.Instance.PlayStartTransition());
        SceneManager.LoadScene("Game");
    }

    public int GetLevel() {
        return level;
    }
}
