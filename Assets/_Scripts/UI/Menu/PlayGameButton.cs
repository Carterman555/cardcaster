using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameButton : GameButton {

    [SerializeField] private bool tutorialButton;

    [SerializeField] private bool noTutorialDebug;

    protected override void OnClick() {
        base.OnClick();

        bool startTutorial = tutorialButton || PlayerPrefs.GetInt("TutorialCompleted") == 0;

        if (noTutorialDebug) {
            startTutorial = false;
        }

        GameSceneManager.Instance.StartGame(startTutorial);

        if (startTutorial) {
            Tutorial.ResetPlayerDied();
        }
    }
}
