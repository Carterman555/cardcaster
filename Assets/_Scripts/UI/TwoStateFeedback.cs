using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MMF_Player))]
public class TwoStateFeedback : MonoBehaviour {

    private MMF_Player feedbackPlayer;
    private int currentState;

    private bool thisIsPlayingFeedback;

    private void Awake() {
        feedbackPlayer = GetComponent<MMF_Player>();

        currentState = 1;
    }

    private void OnEnable() {
        feedbackPlayer.Events.OnComplete.AddListener(OnCompleted);
        feedbackPlayer.Events.OnPlay.AddListener(OnPlay);
    }

    private void OnDisable() {
        feedbackPlayer.Events.OnComplete.RemoveListener(OnCompleted);
        feedbackPlayer.Events.OnPlay.RemoveListener(OnPlay);
    }

    public void ToggleState() {

        if (feedbackPlayer.IsPlaying) {
            return;
        }

        if (currentState == 1) {
            thisIsPlayingFeedback = true;

            feedbackPlayer.SetDirectionTopToBottom();
            feedbackPlayer.PlayFeedbacks();
        }
        else if (currentState == 2) {
            thisIsPlayingFeedback = true;

            feedbackPlayer.SetDirectionBottomToTop();
            feedbackPlayer.PlayFeedbacks();
        }
    }

    public void PlayIfInStateOne() {
        if (feedbackPlayer.IsPlaying) {
            return;
        }

        if (currentState == 1) {
            thisIsPlayingFeedback = true;

            feedbackPlayer.SetDirectionTopToBottom();
            feedbackPlayer.PlayFeedbacks();
        }
    }

    public void PlayIfInStateTwo() {
        if (feedbackPlayer.IsPlaying) {
            return;
        }

        if (currentState == 2) {
            thisIsPlayingFeedback = true;

            feedbackPlayer.SetDirectionBottomToTop();
            feedbackPlayer.PlayFeedbacks();
        }
    }

    public void Revert() {
        feedbackPlayer.Revert();
    }

    public bool IsPlaying() {
        return thisIsPlayingFeedback;
    }

    private void OnPlay() {
        if (!thisIsPlayingFeedback) {
            Debug.LogWarning("Outside script played feedback with two state script!");
        }
    }

    // switch the state when the feedback is done
    private void OnCompleted() {
        if (currentState == 1) {
            currentState = 2;
        }
        else if (currentState == 2) {
            currentState = 1;
        }

        thisIsPlayingFeedback = false;
    }
}
