using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayFeedbackOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private MMF_Player hoverFeedback;

    private bool isEnabled;
    public void Enable() {
        isEnabled = true;
    }
    public void Disable() {
        isEnabled = false;
    }

    private void Awake() {
        Enable();
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (!isEnabled) {
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

        if (!isEnabled) {
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
