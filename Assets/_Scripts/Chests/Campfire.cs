using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour {

    [SerializeField] private TriggerContactTracker playerTracker;

    private bool used;

    [SerializeField] private Animator anim;

    private void OnEnable() {
        playerTracker.OnEnterContact += TryOpenTrashUI;

        used = false;
    }
    private void OnDisable() {
        playerTracker.OnEnterContact -= TryOpenTrashUI;
    }

    private void TryOpenTrashUI(GameObject player) {

        if (used) {
            return;
        }

        FeedbackPlayer.Play("OpenAllCardsPanel");
        TrashCardManager.Instance.Activate();

        used = true;

        anim.SetTrigger("use");
    }
}
