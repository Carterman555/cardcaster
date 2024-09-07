using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayFeedbackOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private MMF_Player hoverFeedback;

    public void OnPointerEnter(PointerEventData eventData) {
        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.SetDirectionTopToBottom();
            hoverFeedback.PlayFeedbacks();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.SetDirectionBottomToTop();
            hoverFeedback.PlayFeedbacks();
        }
    }
}
