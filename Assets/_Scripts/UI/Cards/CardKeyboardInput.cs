using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using static HandCard;

public class CardKeyboardInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    private HandCard handCard;
    private MMFollowTarget followMouse;
    private ShowCardMovement showCardMovement;

    private bool moveCardOnHover;

    private bool mouseDownOnCard;

    private void Awake() {
        handCard = GetComponent<HandCard>();
        followMouse = GetComponent<MMFollowTarget>();
        showCardMovement = GetComponent<ShowCardMovement>();
    }

    private void Start() {
        followMouse.Target = UIFollowMouse.Instance.transform;
    }

    private void OnEnable() {
        SubCancelEvents();

        //... make sure not following the mouse
        followMouse.enabled = false;
        handCard.ShowPlayInput();

        moveCardOnHover = true;
    }

    private void OnDisable() {
        if (handCard.CurrentCardState == CardState.Playing) {
            handCard.CancelCard(followMouse.enabled);
        }

        UnsubCancelEvents();
    }

    private void Update() {
        HandleHotkeyInput();
        HandleMouseInput();
    }

    private void HandleHotkeyInput() {
        bool hotKeyDown = handCard.GetPlayInput().WasPerformedThisFrame();
        bool hotKeyUp = handCard.GetPlayInput().WasReleasedThisFrame();

        if (handCard.CurrentCardState == CardState.ReadyToPlay) {
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
            if (hotKeyDown && !IsPlayingAnyCard()) {

                handCard.OnStartPlayingCard();

                //... show cancel card panel
                FeedbackPlayerOld.Play("CancelCard");

                moveCardOnHover = false;

                // if the card is positional, the hotkey makes it follow the mouse
                if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                    FollowMouse();
                    showCardMovement.OnPositioningCard();
                }

                // if the card is not positional, the hotkey just raises the card
                else {
                    showCardMovement.Show();
                }
            }
        }
        else if (handCard.CurrentCardState == CardState.Playing) {
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

    public void OnPointerEnter(PointerEventData eventData) {
        if (enabled && moveCardOnHover) {
            showCardMovement.Show();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (enabled && moveCardOnHover) {
            showCardMovement.Hide();
        }
    }

    public void OnPointerDown(PointerEventData eventData) {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        if (IsPlayingAnyCard() || handCard.CurrentCardState != CardState.ReadyToPlay) {
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
        moveCardOnHover = false;

        if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard) {
            abilityCard.OnStartPositioningCard(transform);
        }
    }

    private void TryPlayCard() {
        if (setToCancel) {
            handCard.CancelCard(followMouse.enabled);
            moveCardOnHover = true;
        }
        else {
            handCard.TryPlayCard(MouseTracker.Instance.transform.position);
        }

        followMouse.enabled = false;
        handCard.ShowPlayInput();


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
