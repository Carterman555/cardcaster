using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class TradeUIManager : StaticInstance<TradeUIManager>, IInitializable {

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

    [SerializeField] private LocalizedString tradeLocString;

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
        InputManager.OnControlSchemeChanged += TrySelectButton;

        tradeButton.onClick.AddListener(OnTradeButtonClicked);

        this.shopItem = shopItem;
        this.newCard = newCard;

        newCardIcon.sprite = newCard.Sprite;

        GameSceneManager.Instance.StartCoroutine(AllCardsPanel.Instance.MakeCardSelectable(newCard.Rarity));
    }

    public void Deactivate() {
        active = false;

        PanelCardButton.OnClicked_PanelCard -= OnCardClicked;
        SelectButton.OnSelect_PanelCard -= ShowTradeUI;
        InputManager.OnControlSchemeChanged -= TrySelectButton;

        tradeButton.onClick.RemoveListener(OnTradeButtonClicked);

        panelCardToTrade = null;
    }

    public bool IsActive() {
        return active;
    }

    private void TrySelectButton() {
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            GameSceneManager.Instance.StartCoroutine(AllCardsPanel.Instance.MakeCardSelectable(newCard.Rarity));
        }
    }

    private void OnCardClicked(PanelCardButton panelCard) {
        // if clicked the card for the first time
        if (panelCardToTrade != panelCard) {
            panelCardToTrade = panelCard;
            ShowSelectButton(panelCard);
        }
    }

    private void ShowSelectButton(PanelCardButton panelCard) {
        SelectButton.Instance.Show(tradeLocString, panelCard);
    }

    private void ShowTradeUI(PanelCardButton panelCard) {
        FeedbackPlayerReference.Play("TransitionTradeUI");

        SelectButton.Instance.Hide();

        panelCardToTrade = null;

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            tradeButton.Select();
        }

        currentCard = panelCard.GetCard();
        currentCardLocation = panelCard.GetCardLocation();
        currentCardIndex = panelCard.GetCardIndex();

        // reset the icon positions
        currentCardIcon.transform.localPosition = currentCardLocalPosition;
        newCardIcon.transform.localPosition = newCardLocalPosition;

        currentCardIcon.sprite = currentCard.Sprite;
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

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.GainItem);

        float swapDuration = 0.5f;
        currentCardIcon.transform.DOLocalMove(newCardLocalPosition, swapDuration).SetUpdate(true);
        return newCardIcon.transform.DOLocalMove(currentCardLocalPosition, swapDuration).SetUpdate(true);
    }
}
