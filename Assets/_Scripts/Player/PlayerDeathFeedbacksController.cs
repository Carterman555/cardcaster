using MoreMountains.Feedbacks;

public class PlayerDeathFeedbacksController : StaticInstance<PlayerDeathFeedbacksController>
{
    private MMF_Player deathFeedbacks;

    protected override void Awake() {
        base.Awake();
        deathFeedbacks = GetComponent<MMF_Player>();
    }

    private void OnEnable() {
        UpdateShowDeathScreen();
    }

    // if die in tutorial don't show death screen, the tutorial just gets reset
    private void UpdateShowDeathScreen() {

        MMF_Feedbacks showScreen = deathFeedbacks.GetFeedbackOfType<MMF_Feedbacks>("Show Death Screen");
        showScreen.Active = !GameSceneManager.Instance.InTutorial;
    }
}
