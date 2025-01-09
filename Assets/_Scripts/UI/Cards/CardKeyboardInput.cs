using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CardKeyboardInput : MonoBehaviour, IPointerDownHandler {

    private HandCard handCard;
    private MMFollowTarget followMouse;
    private PlayFeedbackOnHover playFeedbackOnHover;

    private bool mouseDownOnCard;

    [SerializeField] protected MMF_Player showCardPlayer;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;
    private InputActionReference playInput;

    private void Awake() {
        handCard = GetComponent<HandCard>();
        followMouse = GetComponent<MMFollowTarget>();
        playFeedbackOnHover = GetComponent<PlayFeedbackOnHover>();
    }

    private void Start() {
        followMouse.Target = UIFollowMouse.Instance.transform;
    }

    private void OnEnable() {
        SubCancelMethods();

        //... make sure not following the mouse
        StopFollowingMouse();

        ShowPlayInput();
    }

    private void OnDisable() {
        UnsubCancelMethods();
    }

    private void Update() {
        HandleHotkeyInput();
        HandleMouseInput();
    }

    private void HandleHotkeyInput() {
        bool hotKeyDown = GetPlayInput().WasPerformedThisFrame();
        bool hotKeyUp = GetPlayInput().WasReleasedThisFrame();

        if (!handCard.CanAffordToPlay()) {
            if (hotKeyDown) {
                handCard.CantPlayShake();
            }
            return;
        }

        // start playing card if hotkey is down and not playing a card
        if (hotKeyDown && !HandCard.IsPlayingAnyCard()) {

            handCard.OnStartPlayingCard();

            //... show cancel card panel
            FeedbackPlayerOld.Play("CancelCard");

            playFeedbackOnHover.enabled = false;

            // if the card is positional, the hotkey makes it follow the mouse
            if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                FollowMouse();
            }

            // if the card is not positional, the hotkey just raises the card
            else {
                showCardPlayer.SetDirectionTopToBottom();
                showCardPlayer.PlayFeedbacks();
            }
        }

        if (hotKeyUp && handCard.IsPlayingCard()) {
            TryPlayCard();
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

        if (HandCard.IsPlayingAnyCard()) {
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
        playFeedbackOnHover.enabled = false;

        if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard) {
            abilityCard.OnStartPositioningCard(transform);
        }
    }

    private void StopFollowingMouse() {
        followMouse.enabled = false;
        playFeedbackOnHover.enabled = true;
        ShowPlayInput();
    }

    private void TryPlayCard() {
        if (setToCancel) {
            handCard.CancelCard();

            //... move back to hand
            StopFollowingMouse();
        }
        else {
            handCard.PlayCard(MouseTracker.Instance.transform.position);
        }

        playFeedbackOnHover.enabled = true;

        //... hide cancel card panel
        FeedbackPlayerOld.PlayInReverse("CancelCard");
    }

    private InputAction GetPlayInput() {
        int cardIndex = handCard.GetIndex();
        if (cardIndex == 0) {
            return playFirstCardInput.action;
        }
        else if (cardIndex == 1) {
            return playSecondCardInput.action;
        }
        else if (cardIndex == 2) {
            return playThirdCardInput.action;
        }
        else {
            Debug.LogError("cardIndex not supported: " + cardIndex);
            return null;
        }
    }

    #region Cancelling

    [SerializeField] private TextMeshProUGUI hotkeyText;

    private bool setToCancel;

    private void SubCancelMethods() {
        CancelCardPanel.OnSetToCancel += SetToCancel;
        CancelCardPanel.OnSetToPlay += SetToPlay;
    }

    private void UnsubCancelMethods() {
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

    private void ShowPlayInput() {
        hotkeyText.text = (handCard.GetIndex() + 1).ToString();
    }

    #endregion Visual


}
