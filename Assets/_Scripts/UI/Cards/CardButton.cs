using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : GameButton {

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI hotkeyText;

    [SerializeField] private Image[] essenceImages;

    [SerializeField] private MMF_Player hoverPlayer;
    [SerializeField] private MMF_Player toHandPlayer;
    [SerializeField] private MMF_Player useCardPlayer;

    private MMFollowTarget followMouse;

    private ScriptableCardBase card;
    private int cardIndex;

    protected override void Awake() {
        base.Awake();
        followMouse = GetComponent<MMFollowTarget>();
    }

    private void Start() {
        followMouse.Target = UIFollowMouse.Instance.transform;
    }

    public void Setup(int cardIndex, Transform deckTransform, Vector3 destination) {
        this.cardIndex = cardIndex;

        MMF_Position hoverMoveFeedback = hoverPlayer.GetFeedbackOfType<MMF_Position>("Move");
        hoverMoveFeedback.InitialPosition = destination;
        hoverMoveFeedback.DestinationPosition = new Vector3(destination.x, hoverMoveFeedback.DestinationPosition.y);

        MMF_Position toHandFeedback = toHandPlayer.GetFeedbackOfType<MMF_Position>("Move To Hand");
        toHandFeedback.InitialPositionTransform = deckTransform;
        toHandFeedback.DestinationPosition = destination;

        useCardPlayer.Events.OnComplete.AddListener(OnUsedCard);

        followMouse.enabled = false;
    }

    private void OnDestroy() {
        useCardPlayer.Events.OnComplete.RemoveListener(OnUsedCard);
    }

    public void OnUsedCard() {
        CardsUIManager.Instance.ReplaceCard(cardIndex);
    }

    public void DrawCard(ScriptableCardBase card) {
        SetCard(card);

        followMouse.enabled = false;

        toHandPlayer.PlayFeedbacks();
    }

    private void Update() {
        bool canAfford = DeckManager.Instance.GetEssence() >= card.GetCost();
        button.interactable = canAfford;

        if (Input.GetKeyDown(KeyCode.Alpha1) && cardIndex == 0 ||
            Input.GetKeyDown(KeyCode.Alpha2) && cardIndex == 1 ||
            Input.GetKeyDown(KeyCode.Alpha3) && cardIndex == 2) {
            FollowMouse();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1) && cardIndex == 0 ||
            Input.GetKeyUp(KeyCode.Alpha2) && cardIndex == 1 ||
            Input.GetKeyUp(KeyCode.Alpha3) && cardIndex == 2) {
            OnClicked();
        }
    }

    protected override void OnClicked() {
        base.OnClicked();

        card.Play();
        useCardPlayer.PlayFeedbacks();

        DeckManager.Instance.UseCard(cardIndex);
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;

        titleText.text = card.GetName();
        hotkeyText.text = (cardIndex + 1).ToString();

        for (int i = 0; i < card.GetCost(); i++) {
            essenceImages[i].enabled = true;
        }
        for (int i = card.GetCost(); i < essenceImages.Length; i++) {
            essenceImages[i].enabled = false;
        }
    }

    public void FollowMouse() {
        followMouse.enabled = true;
    }

    public void ReturnToHand() {
        followMouse.enabled = false;
    }

}
