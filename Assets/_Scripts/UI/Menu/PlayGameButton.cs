using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameButton : GameButton {

    [SerializeField] private bool tutorialButton;

    [SerializeField] private bool noTutorialDebug;

    protected override void OnClick() {
        base.OnClick();

        if (!ES3.KeyExists("TutorialCompleted")) {
            ES3.Save("TutorialCompleted", false);
        }

        bool startTutorial = tutorialButton || !ES3.Load<bool>("TutorialCompleted");

        if (noTutorialDebug) {
            startTutorial = false;
        }

        GameSceneManager.Instance.StartGame(startTutorial);

        if (startTutorial) {
            Tutorial.ResetPlayerDied();
        }
    }
}
