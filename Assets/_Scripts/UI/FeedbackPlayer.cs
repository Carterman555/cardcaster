using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour {

    public static Dictionary<string, FeedbackPlayer> feedbackPlayers = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        feedbackPlayers = new();
    }

    [SerializeField] private string feedbackName;
    private MMF_Player MMFPlayer;

    private void Awake() {
        MMFPlayer = GetComponent<MMF_Player>();

        feedbackPlayers.Add(feedbackName, this);
    }

    public static void Play(string feedbackName) {
        feedbackPlayers[feedbackName].Play();
    }
    public static void PlayInReverse(string popupName) {
        feedbackPlayers[popupName].PlayInReverse();
    }

    public void Play() {
        MMFPlayer.SetDirectionTopToBottom();
        MMFPlayer.PlayFeedbacks();
    }
    public void PlayInReverse() {
        MMFPlayer.SetDirectionBottomToTop();
        MMFPlayer.PlayFeedbacks();
    }
}
