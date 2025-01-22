using MoreMountains.Feedbacks;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayerReference : MonoBehaviour {

    public static Dictionary<string, FeedbackPlayerReference> feedbackPlayers = new();

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

    public static void PlayIfNormal(string feedbackName) {
        feedbackPlayers[feedbackName].GetPlayer().PlayFeedbacksOnlyIfNormalDirection();
    }

    public static void PlayIfReversed(string feedbackName) {
        feedbackPlayers[feedbackName].GetPlayer().PlayFeedbacksOnlyIfReversed();
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

    public void Play() {
        MMFPlayer.PlayFeedbacks();
    }
}
