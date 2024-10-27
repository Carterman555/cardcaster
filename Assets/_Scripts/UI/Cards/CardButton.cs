using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardButton : GameButton, IPointerDownHandler {

    public static event Action<CardButton, ScriptableCardBase> OnAnyCardUsed_ButtonAndCard;
    public static event Action<ScriptableCardBase> OnAnyCardUsed_Card;

    public static event Action<ScriptableCardBase> OnAnyStartPlaying_Card;
    public static event Action<ScriptableCardBase> OnAnyCancel_Card;

    [Header("Feedback Players")]
    [SerializeField] private MMF_Player hoverPlayer;
    [SerializeField] private MMF_Player toHandPlayer;
    [SerializeField] private MMF_Player useCardPlayer;
    [SerializeField] private MMRotationShaker cantPlayShaker;

    // follow mouse
    private MMFollowTarget followMouse;
    private PlayFeedbackOnHover playFeedbackOnHover;
    private bool mouseDownOnCard;

    private ScriptableCardBase card;
    private int cardIndex;

    private static bool playingAnyCard;
    private bool playingCard;

    private bool CanAffordToPlay => DeckManager.Instance.GetEssence() >= card.GetCost();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        playingAnyCard = false;
    }

    protected override void Awake() {
        base.Awake();
        followMouse = GetComponent<MMFollowTarget>();
        playFeedbackOnHover = GetComponent<PlayFeedbackOnHover>();
    }

    private void Start() {
        followMouse.Target = UIFollowMouse.Instance.transform;

        useCardPlayer.Events.OnComplete.AddListener(OnUsedCard);
    }

    public void Setup(int cardIndex, Transform deckTransform, Vector3 position) {
        SetCardIndex(cardIndex);

        SetCardPosition(position);

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.InitialPositionTransform = deckTransform;

        StopFollowingMouse();

        playingCard = false;
    }

    public void SetCardIndex(int cardIndex) {
        this.cardIndex = cardIndex;
        SetHotkeyTextToNum();
    }

    public void SetCardPosition(Vector3 position, bool move = false) {
        // set positions of movement feedbacks
        MMF_Position hoverMoveFeedback = hoverPlayer.GetFeedbackOfType<MMF_Position>("Move");
        hoverMoveFeedback.InitialPosition = position;
        hoverMoveFeedback.DestinationPosition = new Vector3(position.x, hoverMoveFeedback.DestinationPosition.y);

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.DestinationPosition = position;

        handPosition = position;

        // move to that position
        if (move) {
            transform.DOMove(position, duration: 0.2f);
        }
    }

    private void OnDestroy() {
        useCardPlayer.Events.OnComplete.RemoveListener(OnUsedCard);
    }

    public void OnUsedCard() {
        OnAnyCardUsed_ButtonAndCard?.Invoke(this, card);
        OnAnyCardUsed_Card?.Invoke(card);
        playingCard = false;
        playingAnyCard = false;
    }

    public void OnDrawCard(ScriptableCardBase card) {
        SetCard(card);

        StopFollowingMouse();

        toHandPlayer.PlayFeedbacks();
    }

    private void Update() {

        bool hotKeyDown = Input.GetKeyDown(KeyCode.Alpha1) && cardIndex == 0 ||
                Input.GetKeyDown(KeyCode.Alpha2) && cardIndex == 1 ||
                Input.GetKeyDown(KeyCode.Alpha3) && cardIndex == 2;

        bool hotKeyUp = Input.GetKeyUp(KeyCode.Alpha1) && cardIndex == 0 ||
                Input.GetKeyUp(KeyCode.Alpha2) && cardIndex == 1 ||
                Input.GetKeyUp(KeyCode.Alpha3) && cardIndex == 2;

        if (CanAffordToPlay) {

            // start playing card if hotkey is down and not playing another card
            if (hotKeyDown && !playingAnyCard) {

                OnStartPlayingCard();

                playFeedbackOnHover.Disable();

                // if the card is positional, the hotkey makes it follow the mouse
                if (card is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                    FollowMouse();
                }

                // if the card is not positional, the hotkey just raises the card
                else {
                    hoverPlayer.SetDirectionTopToBottom();
                    hoverPlayer.PlayFeedbacks();
                }
            }

            if (hotKeyUp && playingCard) {
                TryPlayCard();
            }

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
        else if (!CanAffordToPlay) {
            if (hotKeyDown) {
                cantPlayShaker.Play();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {

        if (playingAnyCard) {
            return;
        }

        if (!CanAffordToPlay) {
            cantPlayShaker.Play();
            return;
        }

        FollowMouse();
        OnStartPlayingCard();
        mouseDownOnCard = true;
    }

    private void OnStartPlayingCard() {

        playingCard = true;
        playingAnyCard = true;

        // show cancel card panel
        FeedbackPlayer.Play("CancelCard");

        // show the player this will be wasted if the modifier's already active
        TryShowWarning();

        OnAnyStartPlaying_Card?.Invoke(card);
    }

    private void TryPlayCard() {

        if (setToCancel) {
            CancelCard();
        }
        else {
            PlayCard();
        }

        playFeedbackOnHover.Enable();

        // hide cancel card panel
        FeedbackPlayer.PlayInReverse("CancelCard");
    }

    private void PlayCard() {

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        card.TryPlay(mouseWorldPos);
        useCardPlayer.PlayFeedbacks();

        if (card is ScriptableAbilityCardBase) {
            DeckManager.Instance.OnUseAbilityCard(cardIndex);
        }
        else if (card is ScriptableModifierCardBase modifier) {
            DeckManager.Instance.OnUseModifierCard(cardIndex);
        }
    }

    public void FollowMouse() {
        followMouse.enabled = true;
        playFeedbackOnHover.Disable();

        if (card is ScriptableAbilityCardBase abilityCard) {
            abilityCard.OnStartDraggingCard(transform);
        }
    }

    public void StopFollowingMouse() {
        followMouse.enabled = false;
        playFeedbackOnHover.Enable();
        SetHotkeyTextToNum();
    }

    #region Visuals

    [Header("Visual")]
    [SerializeField] private CardImage cardImage;

    [SerializeField] private Image backImage;
    [SerializeField] private Sprite abilityCardBack;
    [SerializeField] private Sprite modifierCardBack;

    [SerializeField] private TextMeshProUGUI hotkeyText;

    public void SetCard(ScriptableCardBase card) {
        this.card = card;
        cardImage.Setup(card);

        SetHotkeyTextToNum();

        if (card is ScriptableAbilityCardBase) {
            backImage.sprite = abilityCardBack;
        }
        else if (card is ScriptableModifierCardBase) {
            backImage.sprite = modifierCardBack;
        }
    }

    private void SetHotkeyTextToNum() {
        hotkeyText.text = (cardIndex + 1).ToString();
    }

    private void TryShowWarning() {
        if (card is ScriptableModifierCardBase modifier) {
            if (AbilityManager.Instance.IsModifierActive(modifier)) {
                hotkeyText.text = "<color=\"red\">Won't Apply!\r\n<size=30>Modifier Already Active</size>";
            }
        }
    }

    #endregion

    #region Handle Cancelling

    private bool setToCancel;

    protected override void OnEnable() {
        base.OnEnable();
        CancelCardPanel.OnSetToCancel += SetToCancel;
        CancelCardPanel.OnSetToPlay += SetToPlay;
    }

    protected override void OnDisable() {
        base.OnDisable();
        CancelCardPanel.OnSetToCancel -= SetToCancel;
        CancelCardPanel.OnSetToPlay -= SetToPlay;
    }

    private void SetToCancel() {
        setToCancel = true;
    }

    private void SetToPlay() {
        setToCancel = false;
    }

    private Vector2 handPosition;

    private void CancelCard() {

        playingCard = false;
        playingAnyCard = false;

        // move back to hand
        StopFollowingMouse();

        float duration = 0.3f;
        transform.DOMove(handPosition, duration);

        // fade out
        MMF_Image fadeFeedback = hoverPlayer.GetFeedbackOfType<MMF_Image>("FadeCard");
        fadeFeedback.RestoreInitialValues();

        OnAnyCancel_Card?.Invoke(card);
    }

    #endregion

    public bool IsPlayingCard() {
        return playingCard;
    }

    public int GetIndex() {
        return cardIndex;
    }
}
