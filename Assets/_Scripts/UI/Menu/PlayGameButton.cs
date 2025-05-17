using UnityEngine;

public class PlayGameButton : GameButton {

    [SerializeField] private bool alwaysTutorial;
    [SerializeField] private bool neverTutorial;

    protected override void OnClick() {
        base.OnClick();

        bool startTutorial;
        if (alwaysTutorial) {
            startTutorial = true;
        }
        else if (neverTutorial) {
            startTutorial = false;
        }
        else {
            startTutorial = !ES3.Load<bool>("TutorialCompleted", false);
        }
        
        GameSceneManager.Instance.StartGame(startTutorial);

        if (startTutorial) {
            Tutorial.ResetPlayerDied();
        }
    }
}
