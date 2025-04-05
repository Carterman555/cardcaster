using MoreMountains.Feedbacks;
using UnityEngine;

public class ToggledPlayFeedbackButton : GameButton {

    [SerializeField] private MMF_Player additiveFeedbackPlayer;
    [SerializeField] private MMF_Player subtractiveFeedbackPlayer;

    private bool inAdditiveState;

    protected override void OnClick() {
        base.OnClick();

        if (additiveFeedbackPlayer.IsPlaying || subtractiveFeedbackPlayer.IsPlaying) {
            return;
        }

        if (inAdditiveState) {
            subtractiveFeedbackPlayer.PlayFeedbacks();
        }
        else {
            additiveFeedbackPlayer.PlayFeedbacksInReverse();
        }

        inAdditiveState = !inAdditiveState;
    }
}
