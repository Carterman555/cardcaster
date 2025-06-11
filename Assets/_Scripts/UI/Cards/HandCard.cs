using DG.Tweening;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class HandCard : MonoBehaviour {

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
    [SerializeField] private MMF_Player usePersistentCardPlayer;
    [SerializeField] private MMF_Player maxPersistentCardPlayer;
    [SerializeField] private MMRotationShaker cantPlayShaker;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;
    [SerializeField] private InputActionReference playForthCardInput;
    [SerializeField] private InputActionReference playFifthCardInput;
    [SerializeField] private InputActionReference playSixthCardInput;

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



    public void OnStartPlayingCard() {

        CurrentCardState = CardState.Playing;
        playingAnyCard = true;

        playingCardThisFrame = true;
        Invoke(nameof(SetPlayingCardThisFrameFalse), Time.deltaTime);

        // show the player this will be wasted if the modifier's already active
        TryShowWarning();

        OnAnyStartPlaying_Card?.Invoke(card);
    }

    // played by mmf_player
    public void TryPlayCard(Vector2 playPosition) {

        if (GameStateManager.Instance.CurrentState != GameState.Game) {
            return;
        }

        if (!card.CanPlay()) {
            return;
        }

        // play before card.TryPlay(playPosition); so it doesn't count as active because of this card
        bool modifierAlreadyActive = card is ScriptableModifierCardBase modCard
                            && AbilityManager.Instance.IsModifierActive(modCard);

        ShowPlayInput();

        card.TryPlay(playPosition);

        CurrentCardState = CardState.Played;

        bool trashingCard = false;

        if (card is ScriptablePersistentCard persistentCard) {

            switch (persistentCard.UpgradeType) {
                case PersistentUpgradeType.NormalUpgrade:
                    usePersistentCardPlayer.PlayFeedbacks();
                    break;
                case PersistentUpgradeType.Dissolve:
                    maxPersistentCardPlayer.PlayFeedbacks();
                    trashingCard = true;
                    break;
                case PersistentUpgradeType.BecomingMaxed:
                    usePersistentCardPlayer.PlayFeedbacks();
                    break;
                case PersistentUpgradeType.AlreadyMaxed:
                    useCardPlayer.PlayFeedbacks();
                    break;
            }
        }
        else {
            useCardPlayer.PlayFeedbacks();
        }

        if (card is ScriptableModifierCardBase modifierCard) {

            bool wontApply = modifierCard.StackType == StackType.NotStackable && modifierAlreadyActive;
            if (wontApply) {
                DeckManager.Instance.DiscardHandCard(cardIndex);
            }
            else {
                DeckManager.Instance.StackHandCard(cardIndex);
            }

            if (modifierCard.StackType == StackType.Stackable || !modifierAlreadyActive) {
                ModifierImage modifierImage = modifierImagePrefab.Spawn(transform.position, Containers.Instance.HUD);
                modifierImage.Setup(modifierCard);
            }
        }
        else if (!trashingCard) {
            DeckManager.Instance.DiscardHandCard(cardIndex);
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.PlayCard);
    }

    private void OnUsedCard() {
        if (GameStateManager.Instance.CurrentState != GameState.Game) {
            return;
        }

        playingAnyCard = false;


        CardsUIManager.Instance.ReturnUsedCard(this);

        ScriptableCardBase usedCard = card;

        bool dissolvingPersistent = card is ScriptablePersistentCard persistentCard && persistentCard.UpgradeType == PersistentUpgradeType.Dissolve;
        if (dissolvingPersistent) {
            DeckManager.Instance.TrashCard(CardLocation.Hand, cardIndex, usingCard: true);
        }

        CardsUIManager.Instance.TryReplaceUsedCard(this);
        CardsUIManager.Instance.TrySpawnCardsToEnd();
        CardsUIManager.Instance.UpdateCardButtons();

        OnAnyCardUsed_Card?.Invoke(usedCard);
    }

    #region Visuals

    [Header("Visual")]
    [SerializeField] private CardImage cardImage;
    [SerializeField] private ModifierImage modifierImagePrefab;

    [SerializeField] private TextMeshProUGUI hotkeyText;

    [SerializeField] private LocalizedString wontApplyLocStr;
    [SerializeField] private LocalizedString alreadyActiveLocStr;

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
                string wontApplyStr = wontApplyLocStr.GetLocalizedString();
                string alreadyActiveStr = alreadyActiveLocStr.GetLocalizedString();

                hotkeyText.text = $"<color=\"red\">{wontApplyStr}\r\n<size=20>{alreadyActiveStr}</size>";
            }
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



    public void CancelCard() {

        CurrentCardState = CardState.Moving;

        // sometimes a card is cancel and another started playing on the same frame, this sets playingAnyCard to false
        // when a card is playing, so make sure a card was not set to playing this frame
        if (!playingCardThisFrame) {
            playingAnyCard = false;
        }

        ShowPlayInput();

        if (card is ScriptableAbilityCardBase abilityCard) {
            abilityCard.Cancel();
        }

        bool positioningCard = (cardKeyboardInput.enabled && cardKeyboardInput.PositioningCard) ||
            (cardControllerInput.enabled && cardControllerInput.MovingCard);

        if (positioningCard) {
            float duration = 0.3f;

            rectTransform.DOKill();
            rectTransform.DOAnchorPos(showPos, duration).SetUpdate(true).OnComplete(() => {
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
        else if (cardIndex == 3) {
            return playForthCardInput.action;
        }
        else if (cardIndex == 4) {
            return playFifthCardInput.action;
        }
        else if (cardIndex == 5) {
            return playSixthCardInput.action;
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

        if (card == null) {
            Debug.LogWarning("Try to play CanAffordToPlay, but card is null!");
            return false;
        }

        return DeckManager.Instance.Essence >= card.Cost;
    }
}
