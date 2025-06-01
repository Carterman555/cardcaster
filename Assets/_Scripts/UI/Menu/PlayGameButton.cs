public class PlayGameButton : GameButton {

    protected override void OnClick() {
        base.OnClick();
        GameSceneManager.Instance.StartGame();
    }
}
