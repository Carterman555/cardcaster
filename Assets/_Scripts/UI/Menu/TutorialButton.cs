public class TutorialButton : GameButton {

    protected override void OnClick() {
        base.OnClick();

        GameSceneManager.Instance.StartGame(tutorial: true);
        Tutorial.ResetPlayerDied();
    }
}
