using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager> {

    [SerializeField] private Animator crossFadeTransition;

    [SerializeField] private float transitionTime = 1f;

    private int level;

    protected override void Awake() {
        base.Awake();
        level = 1;
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"Scene '{scene.name}' loaded - Object ID: {gameObject.GetInstanceID()}");
    }

    public void NextLevel() {
        level++;
        print("next level: " + level);
        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel() {

        crossFadeTransition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("Game");
    }

    public int GetLevel() {
        return level;
    }
}
