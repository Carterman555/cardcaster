using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayFeedbackOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private MMF_Player hoverFeedback;

    [SerializeField] private bool playSFX;
    [ConditionalHide("playSFX")][SerializeField] private AudioClips OnEnterClips;
    [ConditionalHide("playSFX")][SerializeField] private AudioClips OnExitClips;

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

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips);
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

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips);
        }
    }
}
