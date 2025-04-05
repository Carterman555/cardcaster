using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(MMF_Player))]
public class FeedbackPlayerAudio : MonoBehaviour {

    private MMF_Player feedbackPlayer;

    [SerializeField] private bool playPanelSFX;

    [SerializeField] private bool playCustomSFX;
    [ConditionalHide("playCustomSFX")][SerializeField] private AudioClips playClips;
    [ConditionalHide("playCustomSFX")][SerializeField] private AudioClips playInReverseClips;

    [SerializeField] private bool uiSFX = true;

    private void Awake() {
        feedbackPlayer = GetComponent<MMF_Player>();
    }

    private void OnEnable() {
        feedbackPlayer.Events.OnPlay.AddListener(PlayNormalSFX);
    }
    private void OnDisable() {
        feedbackPlayer.Events.OnPlay.RemoveListener(PlayNormalSFX);
    }

    private void OnPlayFeedback() {
        if (feedbackPlayer.Direction == MMFeedbacks.Directions.TopToBottom) {
            PlayNormalSFX();
        }
        else if (feedbackPlayer.Direction == MMFeedbacks.Directions.BottomToTop) {
            PlayInReverseSFX();
        }
    }

    private void PlayNormalSFX() {
        if (playPanelSFX) {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.OpenPanel, uiSound: uiSFX);
        }
        else if (playCustomSFX) {
            AudioManager.Instance.PlaySound(playClips, uiSound: uiSFX);
        }
    }

    private void PlayInReverseSFX() {
        if (playPanelSFX) {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ClosePanel, uiSound: uiSFX);
        }
        else if (playCustomSFX) {
            AudioManager.Instance.PlaySound(playInReverseClips, uiSound: uiSFX);
        }
    }
}
