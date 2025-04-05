using UnityEngine;

public class ShopCard : MonoBehaviour {

    private Interactable interactable;
    private SpriteRenderer spriteRenderer;

    private ScriptableCardBase card;

    [SerializeField] private ChangeColorFromRarity changeShineColor;

    [SerializeField] private bool debugCard;
    [ConditionalHide("debugCard")] [SerializeField] private CardType defaultCard;

    private void Awake() {
        interactable = GetComponent<Interactable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        interactable.OnInteract += OpenAllCardsUI;

        if (debugCard) {
            SetCard(ResourceSystem.Instance.GetCardInstance(defaultCard));
        }
    }
    private void OnDisable() {
        interactable.OnInteract -= OpenAllCardsUI;
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;

        spriteRenderer.sprite = card.Sprite;
        changeShineColor.SetColor(card.Rarity);
    }

    private void OpenAllCardsUI() {

        // make sure the player is set to the correct direction to open
        bool played = FeedbackPlayerReference.GetPlayer("OpenAllCardsPanel").PlayCount > 0;
        if (played) {
            FeedbackPlayerReference.GetPlayer("OpenAllCardsPanel").SetDirectionBottomToTop();
        }

        FeedbackPlayerReference.Play("OpenAllCardsPanel");
        TradeUIManager.Instance.Activate(card, this);
    }
}
