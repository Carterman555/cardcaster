using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPlayer : MonoBehaviour {

    public static Dictionary<string, PopupPlayer> popupPlayers = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        popupPlayers = new();
    }

    [SerializeField] private string popupName;
    private MMF_Player popupPlayer;

    private void Awake() {
        popupPlayer = GetComponent<MMF_Player>();

        popupPlayers.Add(popupName, this);
    }

    public static void Open(string popupName) {
        popupPlayers[popupName].Open();
    }
    public static void Close(string popupName) {
        popupPlayers[popupName].Close();
    }

    public void Open() {
        popupPlayer.SetDirectionTopToBottom();
        popupPlayer.PlayFeedbacks();
    }
    public void Close() {
        popupPlayer.SetDirectionBottomToTop();
        popupPlayer.PlayFeedbacks();
    }
}
