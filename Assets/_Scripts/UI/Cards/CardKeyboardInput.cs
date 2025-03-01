using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardKeyboardInput : MonoBehaviour, IPointerDownHandler {

    private HandCard handCard;
    private MMFollowTarget followMouse;
    private ShowCardMovement showCardMovement;
    private ShowCardOnHover showCardOnHover;

    private bool mouseDownOnCard;

    private void Awake() {
        handCard = GetComponent<HandCard>();
        followMouse = GetComponent<MMFollowTarget>();
        showCardMovement = GetComponent<ShowCardMovement>();
        showCardOnHover = GetComponent<ShowCardOnHover>();
    }

    private void Start() {
        followMouse.Target = UIFollowMouse.Instance.transform;
    }

    private void OnEnable() {
        SubCancelEvents();

        //... make sure not following the mouse
        StopFollowingMouse();
    }

    private void OnDisable() {
        UnsubCancelEvents();
    }

    private void Update() {
        HandleHotkeyInput();
        HandleMouseInput();
    }

    private void HandleHotkeyInput() {
        bool hotKeyDown = handCard.GetPlayInput().WasPerformedThisFrame();
        bool hotKeyUp = handCard.GetPlayInput().WasReleasedThisFrame();

        if (handCard.GetCardState() == HandCard.CardState.ReadyToPlay) {
            if (!handCard.CanAffordToPlay() || !handCard.GetCard().CanPlay()) {
                if (hotKeyDown) {
                    handCard.CantPlayShake();

                    // if tries to play a card that is incompatible with an active ability, show incompatible text
                    if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsIncompatibleAbilityActive()) {
                        StartCoroutine(handCard.ShowIncompatibleText());
                    }
                }
                return;
            }

            // start playing card if hotkey is down and not playing a card
            if (hotKeyDown && !HandCard.IsPlayingAnyCard()) {

                handCard.OnStartPlayingCard();

                //... show cancel card panel
                FeedbackPlayerOld.Play("CancelCard");

                showCardOnHover.enabled = false;

                // if the card is positional, the hotkey makes it follow the mouse
                if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                    FollowMouse();
                }

                // if the card is not positional, the hotkey just raises the card
                else {
                    showCardMovement.MoveUp();
                }
            }
        }
        else if (handCard.GetCardState() == HandCard.CardState.Playing) {
            if (hotKeyUp) {
                TryPlayCard();
            }
        }
    }

    private void HandleMouseInput() {

        if (handCard.CanAffordToPlay()) {
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

        if (HandCard.IsPlayingAnyCard() && handCard.GetCardState() == HandCard.CardState.ReadyToPlay) {
            return;
        }

        if (!handCard.CanAffordToPlay()) {
            handCard.CantPlayShake();
            return;
        }

        FollowMouse();
        handCard.OnStartPlayingCard();
        mouseDownOnCard = true;

        //... show cancel card panel
        FeedbackPlayerOld.Play("CancelCard");
    }

    private void FollowMouse() {
        followMouse.enabled = true;
        showCardOnHover.enabled = false;

        if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard) {
            abilityCard.OnStartPositioningCard(transform);
        }
    }

    private void StopFollowingMouse() {
        followMouse.enabled = false;
        showCardOnHover.enabled = true;
        handCard.ShowPlayInput();
    }

    private void TryPlayCard() {
        if (setToCancel) {
            handCard.CancelCard(followMouse.enabled);
            StopFollowingMouse();
        }
        else {
            handCard.TryPlayCard(MouseTracker.Instance.transform.position);
        }

        showCardOnHover.enabled = true;

        //... hide cancel card panel
        FeedbackPlayerOld.PlayInReverse("CancelCard");
    }

    #region Cancelling


    private bool setToCancel;

    private void SubCancelEvents() {
        CancelCardPanel.OnSetToCancel += SetToCancel;
        CancelCardPanel.OnSetToPlay += SetToPlay;
    }

    private void UnsubCancelEvents() {
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

}
