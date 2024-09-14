using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTrasher : MonoBehaviour {

    [SerializeField] private TriggerContactTracker playerTracker;

    private void OnEnable() {
        playerTracker.OnEnterContact += OpenTrashUI;
    }
    private void OnDisable() {
        playerTracker.OnEnterContact -= OpenTrashUI;
    }

    private void OpenTrashUI(GameObject player) {
        FeedbackPlayer.Play("OpenAllCardsPanel");
    }
}
