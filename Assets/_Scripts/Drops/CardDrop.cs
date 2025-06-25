using DG.Tweening;
using UnityEngine;

public class CardDrop : MonoBehaviour {
    protected ScriptableCardBase scriptableCard;

    private SpriteRenderer spriteRenderer;
    protected Interactable interactable;
    protected SuckMovement suckMovement;

    [SerializeField] private ChangeColorFromRarity changeShineColor;
    [SerializeField] private Transform shine;

    [SerializeField] private bool debugCard;
    [ConditionalHide("debugCard")][SerializeField] private CardType defaultCard; // for testing

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();
        suckMovement = GetComponent<SuckMovement>();

        if (debugCard) {
            SetCard(ResourceSystem.Instance.GetCardInstance(defaultCard));
        }
    }

    private void OnEnable() {
        interactable.OnChangeCanInteract += SetShowCardInfo;
        interactable.OnInteract += OnInteract;
    }
    private void OnDisable() {
        interactable.OnChangeCanInteract -= SetShowCardInfo;
        interactable.OnInteract -= OnInteract;

        shine.DOKill();
        spriteRenderer.DOKill();
    }

    public void SetCard(ScriptableCardBase scriptableCard) {
        this.scriptableCard = scriptableCard;

        spriteRenderer.sprite = scriptableCard.Sprite;

        interactable.enabled = false;

        // spawn visual
        shine.localScale = Vector3.zero;
        shine.DOScale(1f, duration: 1.5f);

        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(1f, duration: 1.5f).OnComplete(() => {

            // because the chestcard already sets the interactable enabled to true
            if (this is not ChestCard) {
                interactable.enabled = true;
            }
        });

        changeShineColor.SetColor(scriptableCard.Rarity);
    }


    private void SetShowCardInfo(bool show) {
        if (show) {
            InteractableInfoUI.Instance.SetCardInfo(scriptableCard);
        }
        else {
            InteractableInfoUI.Instance.RemoveInfo();
        }
    }

    protected virtual void OnInteract() {
        GoToPlayer();
    }

    public void GoToPlayer() {

        interactable.enabled = false;

        //... so it doesn't disappear when the chest does
        transform.SetParent(Containers.Instance.Drops);

        suckMovement.enabled = true;
        suckMovement.Setup(PlayerMovement.Instance.CenterTransform);

        suckMovement.OnReachTarget += ShrinkAndGainCard;
    }

    public void ShrinkAndGainCard() {
        suckMovement.OnReachTarget -= ShrinkAndGainCard;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {
            DeckManager.Instance.GainCard(scriptableCard);

            transform.DOKill();
            gameObject.ReturnToPool();
        });
    }
}
