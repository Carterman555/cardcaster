using UnityEngine;

public class PlayGameButton : GameButton {

    [SerializeField] private bool alwaysTutorial;
    [SerializeField] private bool neverTutorial;

    protected override void OnClick() {
        base.OnClick();

        if (!ES3.KeyExists("TutorialCompleted")) {
            ES3.Save("TutorialCompleted", false);
        }

        bool startTutorial;
        if (alwaysTutorial) {
            startTutorial = true;
        }
        else if (neverTutorial) {
            startTutorial = false;
        }
        else {
            startTutorial = !ES3.Load<bool>("TutorialCompleted");
        }

        GameSceneManager.Instance.StartGame(startTutorial);

        if (startTutorial) {
            Tutorial.ResetPlayerDied();
        }
    }
}
