using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class InteractableInfoUI : MonoBehaviour, IInitializable {

    #region Static Instance

    public static InteractableInfoUI Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    [Header("Card")]
    [SerializeField] private CardImage cardImage;

    [Header("Heal")]
    [SerializeField] private GameObject heal;
    [SerializeField] private TextMeshProUGUI healText;
    [SerializeField] private LocalizedString healLocString;

    [Header("Enchantment")]
    [SerializeField] private EnchantmentInfo enchantmentInfo;

    private ScriptableCardBase cardToShow;
    private ScriptableCardBase cardShowing;

    private bool toShowHeal;
    private bool showingHeal;

    private ScriptableEnchantment enchantmentToShow;
    private ScriptableEnchantment enchantmentShowing;

    private ShowState showState;
    private ShowState delayedCommand;

    public void SetCardInfo(ScriptableCardBase card) {
        cardToShow = card;
        toShowHeal = false;
        enchantmentToShow = null;

        gameObject.SetActive(true);
    }

    public void SetHealInfo() {
        cardToShow = null;
        toShowHeal = true;
        enchantmentToShow = null;

        gameObject.SetActive(true);
    }

    public void SetEnchantmentInfo(ScriptableEnchantment enchantment) {
        cardToShow = null;
        toShowHeal = false;
        enchantmentToShow = enchantment;

        gameObject.SetActive(true);
    }

    public void RemoveInfo() {
        cardToShow = null;
        toShowHeal = false;
        enchantmentToShow = null;
    }

    private void Update() {

        HandleDelayedCommand();

        // everytime the card switches info, it has to be in the bottom pos
        bool hoveringItem = cardToShow != null || toShowHeal || enchantmentToShow != null;
        if (hoveringItem && showState == ShowState.Hidden) {
            SetInfo();
            Show();
        }

        if (!ShowingCorrectInfo() || !hoveringItem) {
            Hide();
        }
    }

    private bool ShowingCorrectInfo() {

        bool cardsMatch = cardToShow == cardShowing;
        bool healsMatch = toShowHeal == showingHeal;
        bool enchantmentsMatch = enchantmentToShow == enchantmentShowing;

        return cardsMatch && healsMatch && enchantmentsMatch;
    }

    private void SetInfo() {

        cardShowing = cardToShow;
        showingHeal = toShowHeal;
        enchantmentShowing = enchantmentToShow;

        if (cardShowing != null) {
            cardImage.gameObject.SetActive(true);
            heal.SetActive(false);
            enchantmentInfo.gameObject.SetActive(false);

            cardImage.Setup(cardToShow);
        }
        else if (showingHeal) {
            cardImage.gameObject.SetActive(false);
            heal.SetActive(true);
            enchantmentInfo.gameObject.SetActive(false);

            healText.text = $"{healLocString.GetLocalizedString()} {ChestHeal.HealAmount}";
        }
        else if (enchantmentShowing != null) {
            cardImage.gameObject.SetActive(false);
            heal.SetActive(false);
            enchantmentInfo.gameObject.SetActive(true);

            enchantmentInfo.Setup(enchantmentShowing.EnchantmentType);
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
