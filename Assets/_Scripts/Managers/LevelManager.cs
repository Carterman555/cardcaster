using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager> {

    private int level;

    protected override void Awake() {
        base.Awake();
        level = 1;
    }

    [Command]
    public void NextLevel() {
        level++;
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel() {
        yield return StartCoroutine(SceneTransitionManager.Instance.PlayStartTransition());
        SceneManager.LoadScene("Game");
    }

    public int GetLevel() {
        return level;
    }
}
