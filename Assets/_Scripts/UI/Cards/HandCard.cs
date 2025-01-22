using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandCard : MonoBehaviour {

    public static event Action<HandCard, ScriptableCardBase> OnAnyCardUsed_ButtonAndCard;
    public static event Action<ScriptableCardBase> OnAnyCardUsed_Card;

    public static event Action<ScriptableCardBase> OnAnyStartPlaying_Card;
    public static event Action<ScriptableCardBase> OnAnyCancel_Card;

    public static event Action<ScriptableCardBase> OnCantAfford_Card;

    [SerializeField] private Vector2 cardStartPos;

    [Header("Feedback Players")]
    [SerializeField] private MMF_Player showCardPlayer;
    [SerializeField] private MMF_Player toHandPlayer;
    [SerializeField] private MMF_Player useCardPlayer;
    [SerializeField] private MMRotationShaker cantPlayShaker;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;

    private ScriptableCardBase card;
    private int cardIndex;

    private static bool playingAnyCard;
    private static bool playingCardThisFrame;
    private bool playingCard;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        playingAnyCard = false;
        playingCardThisFrame = false;
    }

    private void OnEnable() {
        SubInputEvents();

        ControlsChanged();
    }

    private void OnDisable() {
        UnsubInputEvents();
    }

    private void Start() {
        useCardPlayer.Events.OnComplete.AddListener(OnUsedCard);
    }

    private void OnDestroy() {
        useCardPlayer.Events.OnComplete.RemoveListener(OnUsedCard);
    }

    public void Setup(Transform deckTransform, ScriptableCardBase card) {
        playingCard = false;

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.InitialPosition = cardStartPos;

        //... need to wait a frame after setting to hand position before playing feedback for it to go to that
        //... pos
        Invoke(nameof(PlayHandFeedback), Time.deltaTime);

        SetCard(card);
    }

    private void PlayHandFeedback() {
        toHandPlayer.PlayFeedbacks();
    }

    public void SetCardIndex(int cardIndex) {
        this.cardIndex = cardIndex;

        ShowPlayInput();
    }

    // to move to after done moving to hand
    private Vector3 toMovePos;

    private bool waitingForToHandToMove;

    public void SetCardPosition(Vector3 position) {

        // set positions of movement feedbacks
        MMF_Position hoverMoveFeedback = showCardPlayer.GetFeedbackOfType<MMF_Position>("Move");
        hoverMoveFeedback.InitialPosition = position;
        hoverMoveFeedback.DestinationPosition = new Vector3(position.x, hoverMoveFeedback.DestinationPosition.y);

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.DestinationPosition = position;

        handPosition = position;

        // move to that position, if not playing the card
        if (!playingCard) {

            // move right away if in hand pos
            if (!toHandFeedback.IsPlaying) {
                transform.DOKill();
                transform.DOMove(position, duration: 0.2f);
            }

            // wait to move until after done moving to hand
            else {

                toMovePos = position;

                // if not already waiting for the hand player to move the card
                if (!waitingForToHandToMove) {
                    toHandPlayer.Events.OnComplete.AddListener(MoveToPos);
                    waitingForToHandToMove = true;
                }
            }
        }
    }

    private void MoveToPos() {
        transform.DOKill();
        transform.DOMove(toMovePos, duration: 0.2f);

        toMovePos = Vector3.zero;
        waitingForToHandToMove = false;
        toHandPlayer.Events.OnComplete.RemoveListener(MoveToPos);
    }

    private void OnUsedCard() {
        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        OnAnyCardUsed_ButtonAndCard?.Invoke(this, card);
        OnAnyCardUsed_Card?.Invoke(card);
        playingCard = false;
        playingAnyCard = false;
    }

    public void OnStartPlayingCard() {

        playingCard = true;
        playingAnyCard = true;

        playingCardThisFrame = true;
        Invoke(nameof(SetPlayingCardThisFrameFalse), Time.deltaTime);

        // show the player this will be wasted if the modifier's already active
        TryShowWarning();

        OnAnyStartPlaying_Card?.Invoke(card);
    }

    public void PlayCard(Vector2 playPosition) {
        card.TryPlay(playPosition);
        useCardPlayer.PlayFeedbacks();

        if (card is ScriptableAbilityCardBase) {
            DeckManager.Instance.OnUseAbilityCard(cardIndex);
        }
        else if (card is ScriptableModifierCardBase modifier) {
            DeckManager.Instance.OnUseModifierCard(cardIndex);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.PlayCard);
    }

    #region Visuals

    [Header("Visual")]
    [SerializeField] private CardImage cardImage;

    [SerializeField] private TextMeshProUGUI hotkeyText;

    public void SetCard(ScriptableCardBase card) {
        this.card = card;
        cardImage.Setup(card);
    }

    public void CantPlayShake() {
        cantPlayShaker.Play();
        OnCantAfford_Card?.Invoke(card);
    }

    public void ShowPlayInput() {
        hotkeyText.text = InputManager.Instance.GetBindingText(GetPlayInput());
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

    private Vector2 handPosition;

    public void CancelCard() {

        playingCard = false;

        // sometimes a card is cancel and another started playing on the same frame, this sets playingAnyCard to false
        // when a card is playing, so make sure a card was not set to playing this frame
        if (!playingCardThisFrame) {
            playingAnyCard = false;
        }

        if (card is ScriptableAbilityCardBase abilityCard) {
            abilityCard.Cancel();
        }

        float duration = 0.3f;
        transform.DOKill();
        transform.DOMove(handPosition, duration);

        // fade out
        float hoverAlpha = 180f / 255f;
        cardImage.GetComponent<Image>().Fade(hoverAlpha);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.CancelCard);

        OnAnyCancel_Card?.Invoke(card);
    }

    #endregion

    #region Input

    private CardKeyboardInput cardKeyboardInput;
    private CardControllerInput cardControllerInput;

    private void Awake() {
        cardKeyboardInput = GetComponent<CardKeyboardInput>();
        cardControllerInput = GetComponent<CardControllerInput>();
    }

    private void SubInputEvents() {
        InputManager.OnControlsChanged += ControlsChanged;
    }
    private void UnsubInputEvents() {
        InputManager.OnControlsChanged -= ControlsChanged;
    }

    private void ControlsChanged() {

        if (playingCard) {
            CancelCard();
        }

        ShowPlayInput();

        ControlSchemeType controlSchemeType = InputManager.Instance.GetInputScheme();
        if (controlSchemeType == ControlSchemeType.Keyboard) {
            cardKeyboardInput.enabled = true;
            cardControllerInput.enabled = false;
        }
        else if (controlSchemeType == ControlSchemeType.Controller) {
            cardKeyboardInput.enabled = false;
            cardControllerInput.enabled = true;
        }
        else {
            Debug.LogError($"controlSchemeType not found: {controlSchemeType}");
        }
    }

    #endregion

    private void SetPlayingCardThisFrameFalse() {
        playingCardThisFrame = false;
    }

    public bool IsPlayingCard() {
        return playingCard;
    }

    public static bool IsPlayingAnyCard() {
        return playingAnyCard;
    }

    public int GetIndex() {
        return cardIndex;
    }

    public InputAction GetPlayInput() {

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

    public ScriptableCardBase GetCard() {
        return card;
    }

    public bool CanAffordToPlay() {
        return DeckManager.Instance.GetEssence() >= card.GetCost();
    }
}
