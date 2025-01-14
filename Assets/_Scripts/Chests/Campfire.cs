using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour {

    private Interactable interactable;

    [SerializeField] private Animator anim;

    private void Awake() {
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() {
        interactable.OnInteract += OpenTrashUI;
    }
    private void OnDisable() {
        interactable.OnInteract -= OpenTrashUI;
    }

    private void OpenTrashUI() {
        FeedbackPlayer.Play("OpenAllCardsPanel");
        TrashCardManager.Instance.Activate();

        TrashCardManager.OnTrashCard += PutOutFire;
    }

    private void PutOutFire() {
        TrashCardManager.OnTrashCard -= PutOutFire;

        anim.SetTrigger("use");

        interactable.OnInteract -= OpenTrashUI;
        interactable.enabled = false;
    }
}
