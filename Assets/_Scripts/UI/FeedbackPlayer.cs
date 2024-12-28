using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour {

    public static Dictionary<string, FeedbackPlayer> feedbackPlayers = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        feedbackPlayers = new();
    }

    public static MMF_Player GetPlayer(string feedbackName) {
        return feedbackPlayers[feedbackName].GetPlayer();
    }

    public static void Play(string feedbackName) {
        feedbackPlayers[feedbackName].Play();
    }

    [SerializeField] private string feedbackName;
    private MMF_Player MMFPlayer;

    private void Awake() {
        MMFPlayer = GetComponent<MMF_Player>();

        feedbackPlayers.Add(feedbackName, this);
    }

    private void OnDestroy() {
        feedbackPlayers.Remove(feedbackName);
    }
    
    public MMF_Player GetPlayer() {
        return MMFPlayer;
    }

    [ContextMenu("Play")]
    public void Play() {
        MMFPlayer.PlayFeedbacks();
    }
}
