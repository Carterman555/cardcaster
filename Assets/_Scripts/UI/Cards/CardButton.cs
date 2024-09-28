using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardButton : GameButton, IPointerDownHandler {

    [SerializeField] private CardImage cardImage;
    [SerializeField] private TextMeshProUGUI hotkeyText;

    [Header("Feedback Players")]
    [SerializeField] private MMF_Player hoverPlayer;
    [SerializeField] private MMF_Player toHandPlayer;
    [SerializeField] private MMF_Player useCardPlayer;

    // follow mouse
    private MMFollowTarget followMouse;
    private PlayFeedbackOnHover playFeedbackOnHover;
    private bool mouseDownOnCard;

    private ScriptableCardBase card;
    private int cardIndex;

    protected override void Awake() {
        base.Awake();
        followMouse = GetComponent<MMFollowTarget>();
        playFeedbackOnHover = GetComponent<PlayFeedbackOnHover>();
    }

    private void Start() {
        followMouse.Target = UIFollowMouse.Instance.transform;
    }

    public void Setup(int cardIndex, Transform deckTransform, Vector3 destination) {
        this.cardIndex = cardIndex;

        hotkeyText.text = (cardIndex + 1).ToString();

        MMF_Position hoverMoveFeedback = hoverPlayer.GetFeedbackOfType<MMF_Position>("Move");
        hoverMoveFeedback.InitialPosition = destination;
        hoverMoveFeedback.DestinationPosition = new Vector3(destination.x, hoverMoveFeedback.DestinationPosition.y);

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.InitialPositionTransform = deckTransform;
        toHandFeedback.DestinationPosition = destination;

        useCardPlayer.Events.OnComplete.AddListener(OnUsedCard);

        StopFollowingMouse();
    }

    private void OnDestroy() {
        useCardPlayer.Events.OnComplete.RemoveListener(OnUsedCard);
    }

    public void OnUsedCard() {
        CardsUIManager.Instance.TryReplaceCard(cardIndex);
    }

    public void OnDrawCard(ScriptableCardBase card) {
        SetCard(card);

        StopFollowingMouse();

        toHandPlayer.PlayFeedbacks();
    }

    private void Update() {
        bool canAfford = DeckManager.Instance.GetEssence() >= card.GetCost();
        button.interactable = canAfford;

        // handle hotkeys
        if (Input.GetKeyDown(KeyCode.Alpha1) && cardIndex == 0 ||
            Input.GetKeyDown(KeyCode.Alpha2) && cardIndex == 1 ||
            Input.GetKeyDown(KeyCode.Alpha3) && cardIndex == 2) {

            if (card is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                FollowMouse();
            }
            else {
                hoverPlayer.PlayFeedbacks();
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha1) && cardIndex == 0 ||
            Input.GetKeyUp(KeyCode.Alpha2) && cardIndex == 1 ||
            Input.GetKeyUp(KeyCode.Alpha3) && cardIndex == 2) {
            PlayCard();
        }

        // the reason for this instead of using on mouse click is because this:
        //      the player can be dragging the card, then quickly mouse the mouse and release and it won't count a click
        //      because the mouse is not on the card
        if (mouseDownOnCard) {
            if (Input.GetMouseButtonUp(0)) {
                PlayCard();
                mouseDownOnCard = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        FollowMouse();
        mouseDownOnCard = true;
    }

    private void PlayCard() {

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        card.Play(mouseWorldPos);
        useCardPlayer.PlayFeedbacks();

        if (card is ScriptableAbilityCardBase) {
            DeckManager.Instance.OnUseAbilityCard(cardIndex);
        }
        else if (card is ScriptableModifierCardBase) {
            DeckManager.Instance.OnUseModifierCard(cardIndex);
        }
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;
        cardImage.Setup(card);
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
    }
}
