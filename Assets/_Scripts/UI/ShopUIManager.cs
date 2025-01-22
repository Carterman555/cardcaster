using DG.Tweening;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : StaticInstance<ShopUIManager>, IInitializable {

    public void Initialize() => Instance = this;

    [SerializeField] private Image currentCardIcon;
    [SerializeField] private Image newCardIcon;

    private Vector2 currentCardLocalPosition;
    private Vector2 newCardLocalPosition;

    private ScriptableCardBase currentCard;
    private CardLocation currentCardLocation;
    private int currentCardIndex;

    [SerializeField] private Button tradeButton;

    private ScriptableCardBase newCard;
    private ShopCard shopItem;

    private bool active;

    private PanelCardButton panelCardToTrade;

    #region Open Trade UI

    protected override void Awake() {
        base.Awake();
        currentCardLocalPosition = currentCardIcon.transform.localPosition;
        newCardLocalPosition = newCardIcon.transform.localPosition;
    }

    public void Activate(ScriptableCardBase newCard, ShopCard shopItem) {
        active = true;

        PanelCardButton.OnClicked_PanelCard += OnCardClicked;
        SelectButton.OnSelect_PanelCard += ShowTradeUI;

        tradeButton.onClick.AddListener(OnTradeButtonClicked);

        this.shopItem = shopItem;
        this.newCard = newCard;

        newCardIcon.sprite = newCard.GetSprite();

        DisableLesserRarities(newCard.GetRarity());

        //... can select cards if trading card
        AllCardsPanel.Instance.TrySetupControllerCardSelection();
    }

    public void Deactivate() {
        active = false;

        PanelCardButton.OnClicked_PanelCard -= OnCardClicked;
        SelectButton.OnSelect_PanelCard -= ShowTradeUI;

        tradeButton.onClick.RemoveListener(OnTradeButtonClicked);

        panelCardToTrade = null;
    }

    public bool IsActive() {
        return active;
    }

    private void OnCardClicked(PanelCardButton panelCard) {
        // if clicked the card for the first time
        if (panelCardToTrade != panelCard) {
            panelCardToTrade = panelCard;
            ShowSelectButton(panelCard);
        }
        // if clicked the card a second time, trade it
        else {
            ShowTradeUI(panelCard);
        }
    }

    private void ShowSelectButton(PanelCardButton panelCard) {
        Vector2 offset = new Vector2(0f, 205f);
        Vector2 position = (Vector2)panelCard.transform.position + offset;

        SelectButton.Instance.Show("Trade", position, panelCard);
    }

    private void ShowTradeUI(PanelCardButton panelCard) {
        FeedbackPlayerReference.Play("TransitionTradeUI");

        SelectButton.Instance.Hide();

        panelCardToTrade = null;

        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Controller) {
            tradeButton.Select();
        }

        currentCard = panelCard.GetCard();
        currentCardLocation = panelCard.GetCardLocation();
        currentCardIndex = panelCard.GetCardIndex();

        // reset the icon positions
        currentCardIcon.transform.localPosition = currentCardLocalPosition;
        newCardIcon.transform.localPosition = newCardLocalPosition;

        currentCardIcon.sprite = currentCard.GetSprite();
    }

    // only cards that are of equal rarity or rarer can be traded
    private void DisableLesserRarities(Rarity newCardRarity) {
        PanelCardButton[] allPanelCards = FindObjectsOfType<PanelCardButton>();
        foreach (PanelCardButton panelCard in allPanelCards) {

            // if panel card is more common than item trying to trade
            if (panelCard.GetCard().GetRarity() < newCardRarity) {
                panelCard.GetComponent<Button>().interactable = false;
            }
        }
    }

    #endregion


    private void OnTradeButtonClicked() {

        //... can't close the trade when already started, the player is set to can play in the CloseTradeUI player
        FeedbackPlayerReference.GetPlayer("TransitionTradeUI").CanPlay = false;

        SwapIcons().OnComplete(() => {
            FeedbackPlayerOld.Play("CloseTradeUI");

            // replace the shop item with player's card
            shopItem.SetCard(currentCard);

            // replace the player's card with shop item card
            DeckManager.Instance.ReplaceCard(currentCardLocation, currentCardIndex, newCard);
        });
    }

    private Tween SwapIcons() {
        float swapDuration = 0.5f;
        currentCardIcon.transform.DOLocalMove(newCardLocalPosition, swapDuration).SetUpdate(true);
        return newCardIcon.transform.DOLocalMove(currentCardLocalPosition, swapDuration).SetUpdate(true);
    }
}
