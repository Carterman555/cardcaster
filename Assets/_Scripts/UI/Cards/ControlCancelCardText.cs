using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlCancelCardText : MonoBehaviour {

    private MMF_Player feedbackPlayer;

    private void Awake() {
        feedbackPlayer = GetComponent<MMF_Player>();
    }

    private void Update() {

        // only show the cancel text if using controller
        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Keyboard) {
            bool showing = feedbackPlayer.InSecondState();
            if (showing) {
                feedbackPlayer.PlayFeedbacks(); // hide
            }
            return;
        }

        // show text when any card is playing and hide text when no card is playing
        if (HandCard.IsPlayingAnyCard()) {
            bool hiding = feedbackPlayer.InFirstState();
            if (hiding) {
                feedbackPlayer.PlayFeedbacks(); // show text
            }
        }
        else {
            bool showing = feedbackPlayer.InSecondState();
            if (showing) {
                feedbackPlayer.PlayFeedbacks(); // hide text
            }
        }
    }
}
