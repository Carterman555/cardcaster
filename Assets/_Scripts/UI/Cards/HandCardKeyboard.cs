using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandCardKeyboard : HandCard, IPointerDownHandler {

    private MMFollowTarget followMouse;
    private PlayFeedbackOnHover playFeedbackOnHover;

    private bool mouseDownOnCard;

    protected override void Awake() {
        base.Awake();
        followMouse = GetComponent<MMFollowTarget>();
        playFeedbackOnHover = GetComponent<PlayFeedbackOnHover>();
    }

    protected override void Start() {
        base.Start();
        followMouse.Target = UIFollowMouse.Instance.transform;
    }

    public override void Setup(Transform deckTransform, ScriptableCardBase card) {
        base.Setup(deckTransform, card);

        //... make sure not following the mouse
        StopFollowingMouse();
    }

    private void Update() {
        HandleHotkeyInput();
        HandleMouseInput();
    }

    private void HandleHotkeyInput() {
        bool hotKeyDown = GetPlayInput().WasPerformedThisFrame();
        bool hotKeyUp = GetPlayInput().WasReleasedThisFrame();

        if (CanAffordToPlay) {

            // start playing card if hotkey is down and not playing another card
            if (hotKeyDown && !playingAnyCard) {

                OnStartPlayingCard();

                //... show cancel card panel
                FeedbackPlayerOld.Play("CancelCard");

                playFeedbackOnHover.Disable();

                // if the card is positional, the hotkey makes it follow the mouse
                if (GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                    FollowMouse();
                }

                // if the card is not positional, the hotkey just raises the card
                else {
                    showCardPlayer.SetDirectionTopToBottom();
                    showCardPlayer.PlayFeedbacks();
                }
            }

            if (hotKeyUp && playingCard) {
                TryPlayCard();
            }

            
        }
        else if (!CanAffordToPlay) {
            if (hotKeyDown) {
                CantPlayShake();
            }
        }
    }

    private void HandleMouseInput() {

        if (CanAffordToPlay) {
            // the reason for this instead of using on mouse click is because this:
            //      the player can be dragging the card, then quickly mouse the mouse and release and it won't count a click
            //      because the mouse is not on the card
            if (mouseDownOnCard) {
                if (Input.GetMouseButtonUp(0)) {
                    TryPlayCard();
                    mouseDownOnCard = false;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        if (playingAnyCard) {
            return;
        }

        if (!CanAffordToPlay) {
            CantPlayShake();
            return;
        }

        FollowMouse();
        OnStartPlayingCard();
        mouseDownOnCard = true;

        //... show cancel card panel
        FeedbackPlayerOld.Play("CancelCard");
    }

    private void FollowMouse() {
        followMouse.enabled = true;
        playFeedbackOnHover.Disable();

        if (GetCard() is ScriptableAbilityCardBase abilityCard) {
            abilityCard.OnStartPositioningCard(transform);
        }
    }

    private void StopFollowingMouse() {
        followMouse.enabled = false;
        playFeedbackOnHover.Enable();
        ShowPlayInput();
    }

    private void TryPlayCard() {
        if (setToCancel) {
            CancelCard();

            //... move back to hand
            StopFollowingMouse();
        }
        else {
            PlayCard(MouseTracker.Instance.transform.position);
        }

        playFeedbackOnHover.Enable();

        //... hide cancel card panel
        FeedbackPlayerOld.PlayInReverse("CancelCard");
    }

    #region Cancelling

    private bool setToCancel;

    private void OnEnable() {
        CancelCardPanel.OnSetToCancel += SetToCancel;
        CancelCardPanel.OnSetToPlay += SetToPlay;
    }

    private void OnDisable() {
        CancelCardPanel.OnSetToCancel -= SetToCancel;
        CancelCardPanel.OnSetToPlay -= SetToPlay;
    }

    private void SetToCancel() {
        setToCancel = true;
    }

    private void SetToPlay() {
        setToCancel = false;
    }

    #endregion

    #region Visual

    protected override void ShowPlayInput() {
        base.ShowPlayInput();
        hotkeyText.text = (GetIndex() + 1).ToString();
    }

    #endregion Visual
}
