using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayFeedbackOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private MMF_Player hoverFeedback;

    [SerializeField] private bool playSFX;
    [ConditionalHide("playSFX")][SerializeField] private AudioClips OnEnterClips;
    [ConditionalHide("playSFX")][SerializeField] private AudioClips OnExitClips;

    //... so component can be enabled and disabled
    private void OnEnable() { }

    public void OnPointerEnter(PointerEventData eventData) {

        if (!enabled) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.SetDirectionTopToBottom();
            hoverFeedback.PlayFeedbacks();
        }

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips, uiSound: true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (!enabled) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.SetDirectionBottomToTop();
            hoverFeedback.PlayFeedbacks();
        }

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips);
        }
    }
}
