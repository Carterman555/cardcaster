using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class ChestItemInfoUI : MonoBehaviour, IInitializable {

    #region Static Instance

    public static ChestItemInfoUI Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    [SerializeField] private CardImage cardImage;
    [SerializeField] private GameObject heal;
    [SerializeField] private TextMeshProUGUI healText;

    [SerializeField] private LocalizedString healLocString;

    private ItemInfo itemInfoShowing;
    private ItemInfo itemInfoToShow;

    private ShowState showState;
    private ShowState delayedCommand;

    public void SetCardInfo(ScriptableCardBase card) {
        itemInfoToShow = new() { Heal = false, Card = card };
        gameObject.SetActive(true);
    }

    public void SetHealInfo() {
        itemInfoToShow = new() { Heal = true, Card = null };
        gameObject.SetActive(true);
        healText.text = $"{healLocString.GetLocalizedString()} {ChestHeal.HealAmount}";
    }

    public void RemoveInfo() {
        itemInfoToShow = null;
    }

    private void Update() {

        HandleDelayedCommand();

        // everytime the card switches info, it has to be in the bottom pos
        bool hoveringItem = itemInfoToShow != null;
        if (hoveringItem && showState == ShowState.Hidden) {
            SetInfo(itemInfoToShow);
            Show();
        }

        if (!IsPanelShowingInfo()) {
            Hide();
        }
    }

    private bool IsPanelShowingInfo() {

        if (itemInfoToShow == null || itemInfoShowing == null) {
            return false;
        }

        bool sameHeal = itemInfoToShow.Heal == itemInfoShowing.Heal;

        // sameCard is true if the card types are the same or they're both null
        bool sameCard = false;
        if (itemInfoToShow.Card != null && itemInfoShowing.Card != null) {
            sameCard = itemInfoToShow.Card.CardType == itemInfoShowing.Card.CardType;
        }
        else if (itemInfoToShow.Card == null && itemInfoToShow.Card == null) {
            sameCard = true;
        }

        return sameHeal && sameCard;
    }

    private void SetInfo(ItemInfo itemInfo) {

        itemInfoShowing = itemInfo;

        cardImage.gameObject.SetActive(!itemInfo.Heal);
        heal.SetActive(itemInfo.Heal);

        if (!itemInfo.Heal) {
            cardImage.Setup(itemInfo.Card);
        }
    }

    #region Movement

    

    private RectTransform rectTransform;

    [SerializeField] private Vector2 hidePos, showPos;

    const float duration = 0.2f;
    const float fade = 0.7f;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        delayedCommand = ShowState.None;
        showState = ShowState.Hidden;
    }

    public void Show() {

        if (!enabled) {
            return;
        }

        if (showState == ShowState.Hidden) {
            showState = ShowState.Moving;
            
            rectTransform.DOAnchorPos(showPos, duration).OnComplete(() => {
                showState = ShowState.Showing;
            });
        }
        else if (showState == ShowState.Moving) {
            delayedCommand = ShowState.Showing;
        }
    }

    public void Hide() {

        if (!enabled) {
            return;
        }

        if (showState == ShowState.Showing) {
            showState = ShowState.Moving;

            rectTransform.DOAnchorPos(hidePos, duration).OnComplete(() => {
                showState = ShowState.Hidden;
            });
        }
        else if (showState == ShowState.Moving) {
            delayedCommand = ShowState.Hidden;
        }
    }

    private void HandleDelayedCommand() {

        if (delayedCommand != ShowState.None) {
            if (showState != ShowState.Moving) {

                if (delayedCommand == ShowState.Hidden) {
                    Hide();
                }
                else if (delayedCommand == ShowState.Showing) {
                    Show();
                }

                delayedCommand = ShowState.None;
            }
        }
    }

    #endregion
}

public class ItemInfo {
    public bool Heal;
    public ScriptableCardBase Card;
}
