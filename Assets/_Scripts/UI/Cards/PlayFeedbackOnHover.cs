using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayFeedbackOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private MMF_Player hoverFeedback;

    private bool playFeedback;
    public void Enable() {
        playFeedback = true;
    }
    public void Disable() {
        playFeedback = false;
    }

    private void Awake() {
        Enable();
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (!playFeedback) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.SetDirectionTopToBottom();
            hoverFeedback.PlayFeedbacks();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (!playFeedback) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.SetDirectionBottomToTop();
            hoverFeedback.PlayFeedbacks();
        }
    }
}
