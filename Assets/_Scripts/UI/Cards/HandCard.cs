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

    public static event Action<HandCard> OnAnyCardUsed_Button;
    public static event Action<ScriptableCardBase> OnAnyCardUsed_Card;

    public static event Action<ScriptableCardBase> OnAnyStartPlaying_Card;
    public static event Action<ScriptableCardBase> OnAnyCancel_Card;

    public static event Action<ScriptableCardBase> OnCantAfford_Card;

    public CardState CurrentCardState;
    public enum CardState { Moving, ReadyToPlay, Playing, Played }

    [SerializeField] private Vector2 cardStartPos;

    private RectTransform rectTransform;

    private ShowCardMovement showCardMovement;
    private Vector3 showPos;

    [Header("Feedback Players")]
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        playingAnyCard = false;
        playingCardThisFrame = false;
    }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
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

        CurrentCardState = CardState.Moving;

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
        CurrentCardState = CardState.ReadyToPlay;

        toHandPlayer.Events.OnComplete.RemoveListener(SetStateToReady);
    }

    public void SetCardIndex(int cardIndex) {
        this.cardIndex = cardIndex;

        ShowPlayInput();
    }

    // to move to after done moving to hand
    private Vector3 toMovePos;
    private bool waitingForToHandToMove;

    public void SetCardPosition(Vector3 position) {

        showPos = new(position.x, 200f);
        showCardMovement.Setup(position, showPos);

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.DestinationPosition = position;

        // move to that position, if not playing the card and card is not moving
        if (CurrentCardState == CardState.ReadyToPlay) {
            CurrentCardState = CardState.Moving;

            rectTransform.DOKill();
            rectTransform.DOAnchorPos(position, duration: 0.2f).SetUpdate(true).OnComplete(() => {
                CurrentCardState = CardState.ReadyToPlay;
            });
        }

        // wait to move until after done moving to hand
        else if (CurrentCardState == CardState.Moving) {
            toMovePos = position;

            // if not already waiting for the hand player to move the card
            if (!waitingForToHandToMove) {
                StartCoroutine(MoveWhenReady());
                waitingForToHandToMove = true;
            }
        }
    }

    private IEnumerator MoveWhenReady() {
        while (CurrentCardState != CardState.ReadyToPlay) {
            yield return null;
        }

        MoveToPos();
    }

    private void MoveToPos() {
        waitingForToHandToMove = false;
        CurrentCardState = CardState.Moving;

        rectTransform.DOKill();
        rectTransform.DOAnchorPos(toMovePos, duration: 0.2f).OnComplete(() => {
            CurrentCardState = CardState.ReadyToPlay;
        });

        toMovePos = Vector3.zero;
    }

    private void OnUsedCard() {
        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        OnAnyCardUsed_Card?.Invoke(card);
        OnAnyCardUsed_Button?.Invoke(this);
        playingAnyCard = false;
    }

    public void OnStartPlayingCard() {

        CurrentCardState = CardState.Playing;
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

        CurrentCardState = CardState.Played;

        useCardPlayer.PlayFeedbacks();

        bool stackCard = card is ScriptableModifierCardBase;
        DeckManager.Instance.OnUseCard(cardIndex, stackCard);

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

    public void CancelCard(bool positioningCard) {

        CurrentCardState = CardState.Moving;

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

            rectTransform.DOKill();
            rectTransform.DOAnchorPos(showPos, duration).OnComplete(() => {
                showCardMovement.Hide();
            });
        }
        else {
            showCardMovement.Hide();
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

        ShowPlayInput();

        ControlSchemeType controlSchemeType = InputManager.Instance.GetControlScheme();
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
        return DeckManager.Instance.Essence >= card.Cost;
    }
}
