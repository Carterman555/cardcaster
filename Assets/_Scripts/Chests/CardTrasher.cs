using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTrasher : MonoBehaviour {

    [SerializeField] private TriggerContactTracker playerTracker;
    [SerializeField] private MMF_Player openAllCardsPlayer;

    private void OnEnable() {
        playerTracker.OnEnterContact += OpenTrashUI;
    }
    private void OnDisable() {
        playerTracker.OnEnterContact -= OpenTrashUI;
    }

    private void OpenTrashUI(GameObject player) {
        openAllCardsPlayer.PlayFeedbacks();
    }
}
