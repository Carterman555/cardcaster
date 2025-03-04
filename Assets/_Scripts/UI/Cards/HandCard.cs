using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static HandCard;

public class HandCard : MonoBehaviour {

    public static event Action<HandCard, ScriptableCardBase> OnAnyCardUsed_ButtonAndCard;
    public static event Action<ScriptableCardBase> OnAnyCardUsed_Card;

    public static event Action<ScriptableCardBase> OnAnyStartPlaying_Card;
    public static event Action<ScriptableCardBase> OnAnyCancel_Card;

    public static event Action<ScriptableCardBase> OnCantAfford_Card;

    [SerializeField] private Vector2 cardStartPos;
    private ShowCardMovement showCardMovement;

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
    private CardState cardState;

    public enum CardState { Moving, ReadyToPlay, Playing, Played }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        playingAnyCard = false;
        playingCardThisFrame = false;
    }

    private void Awake() {
        showCardMovement = GetComponent<ShowCardMovement>();

        cardKeyboardInput = GetComponent<CardKeyboardInput>();
        cardControllerInput = GetComponent<CardControllerInput>();
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
        SceneManager.sceneLoaded += SetPlayingAnyCardFalse;
    }

    private void OnDestroy() {
        useCardPlayer.Events.OnComplete.RemoveListener(OnUsedCard);
        SceneManager.sceneLoaded -= SetPlayingAnyCardFalse;
    }

    public void Setup(Transform deckTransform, ScriptableCardBase card) {

        cardState = CardState.Moving;

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.InitialPosition = cardStartPos;

        toHandPlayer.Events.OnComplete.AddListener(SetStateToReady);

        //... need to wait a frame after setting to hand position before playing feedback for it to go to that
        //... pos
        Invoke(nameof(PlayHandFeedback), Time.deltaTime);

        SetCard(card);
    }

    private void PlayHandFeedback() {
        toHandPlayer.PlayFeedbacks();
    }

    private void SetStateToReady() {
        cardState = CardState.ReadyToPlay;

        toHandPlayer.Events.OnComplete.RemoveListener(SetStateToReady);
        showCardPlayer.Events.OnComplete.RemoveListener(SetStateToReady);
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

        // move to that position, if not playing the card and card is not moving
        if (cardState == CardState.ReadyToPlay) {
            cardState = CardState.Moving;

            transform.DOKill();
            transform.DOMove(position, duration: 0.2f).OnComplete(() => {
                cardState = CardState.ReadyToPlay;
            });
        }

        // wait to move until after done moving to hand
        else if (cardState == CardState.Moving) {
            toMovePos = position;

            // if not already waiting for the hand player to move the card
            if (!waitingForToHandToMove) {
                StartCoroutine(MoveWhenReady());
                waitingForToHandToMove = true;
            }
        }
    }

    private IEnumerator MoveWhenReady() {
        while (cardState != CardState.ReadyToPlay) {
            yield return null;
        }

        MoveToPos();
    }

    private void MoveToPos() {
        waitingForToHandToMove = false;
        cardState = CardState.Moving;

        transform.DOKill();
        transform.DOMove(toMovePos, duration: 0.2f).OnComplete(() => {
            cardState = CardState.ReadyToPlay;
        });

        toMovePos = Vector3.zero;
    }

    private void OnUsedCard() {
        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        OnAnyCardUsed_ButtonAndCard?.Invoke(this, card);
        OnAnyCardUsed_Card?.Invoke(card);
        playingAnyCard = false;
    }

    public void OnStartPlayingCard() {

        cardState = CardState.Playing;
        playingAnyCard = true;

        playingCardThisFrame = true;
        Invoke(nameof(SetPlayingCardThisFrameFalse), Time.deltaTime);

        // show the player this will be wasted if the modifier's already active
        TryShowWarning();

        OnAnyStartPlaying_Card?.Invoke(card);
    }

    public void TryPlayCard(Vector2 playPosition) {

        if (!card.CanPlay()) {
            return;
        }

        card.TryPlay(playPosition);

        cardState = CardState.Played;

        useCardPlayer.PlayFeedbacks();

        if (card is ScriptableAbilityCardBase) {
            DeckManager.Instance.OnUseAbilityCard(cardIndex);
        }
        else if (card is ScriptableModifierCardBase modifier) {
            DeckManager.Instance.OnUseModifierCard(cardIndex);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.PlayCard, uiSound: false);
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

        if (card is ScriptableModifierCardBase modifierCard) {
            if (modifierCard.StackType == StackType.NotStackable && AbilityManager.Instance.IsModifierActive(modifierCard)) {
                hotkeyText.text = "<color=\"red\">Won't Apply!\r\n<size=20>Modifier Already Active</size>";
            }
        }

        if (card is ScriptableAbilityCardBase abilityCard) {

        }
    }

    [SerializeField] private TextMeshProUGUI incompatitableText;

    [ContextMenu("Show Incompatible")]
    public IEnumerator ShowIncompatibleText() {

        incompatitableText.gameObject.SetActive(true);

        Color fadedRed = Color.red;
        fadedRed.a = 0f;
        incompatitableText.color = fadedRed;
        incompatitableText.DOFade(1f, duration: 0.2f);

        float timeToShow = 1f;
        yield return new WaitForSeconds(timeToShow);

        incompatitableText.DOFade(0f, duration: 0.2f).OnComplete(() => {
            incompatitableText.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Handle Cancelling

    private Vector2 handPosition;

    public void CancelCard(bool positioningCard) {

        cardState = CardState.Moving;

        // sometimes a card is cancel and another started playing on the same frame, this sets playingAnyCard to false
        // when a card is playing, so make sure a card was not set to playing this frame
        if (!playingCardThisFrame) {
            playingAnyCard = false;
        }

        if (card is ScriptableAbilityCardBase abilityCard) {
            abilityCard.Cancel();
        }

        if (positioningCard) {
            float duration = 0.3f;
            Vector2 showPos = showCardPlayer.GetFeedbackOfType<MMF_Position>().DestinationPosition;

            transform.DOKill();
            transform.DOMove(showPos, duration).OnComplete(() => {
                showCardPlayer.SetDirectionTopToBottom();
                showCardMovement.MoveDown();

                showCardPlayer.Events.OnComplete.AddListener(SetStateToReady);
            });
        }
        else {
            showCardMovement.MoveDown();
            showCardPlayer.Events.OnComplete.AddListener(SetStateToReady);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.CancelCard);

        OnAnyCancel_Card?.Invoke(card);
    }

    #endregion

    #region Input

    private CardKeyboardInput cardKeyboardInput;
    private CardControllerInput cardControllerInput;

    private void SubInputEvents() {
        InputManager.OnControlsChanged += ControlsChanged;
    }
    private void UnsubInputEvents() {
        InputManager.OnControlsChanged -= ControlsChanged;
    }

    private void ControlsChanged() {

        if (cardState == CardState.Playing) {
            CancelCard(false);
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

    private void SetPlayingAnyCardFalse(Scene arg0, LoadSceneMode arg1) {
        playingAnyCard = false;
    }

    private void SetPlayingCardThisFrameFalse() {
        playingCardThisFrame = false;
    }

    public CardState GetCardState() {
        return cardState;
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
