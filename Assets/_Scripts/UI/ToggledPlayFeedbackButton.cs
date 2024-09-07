using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ToggledPlayFeedbackButton : GameButton {

    [SerializeField] private MMF_Player additiveFeedbackPlayer;
    [SerializeField] private MMF_Player subtractiveFeedbackPlayer;

    private bool inAdditiveState;

    protected override void OnClicked() {
        base.OnClicked();

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
