using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardUnlockDrop : MonoBehaviour {
    protected ScriptableCardBase scriptableCard;

    private SpriteRenderer spriteRenderer;
    protected Interactable interactable;

    [SerializeField] private float fade = 0.7f;
    [SerializeField] private ChangeColorFromRarity changeShineColor;
    [SerializeField] private Transform shine;

    [SerializeField] private bool debugCard;
    [ConditionalHide("debugCard")][SerializeField] private CardType defaultCard; // for testing

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactable = GetComponent<Interactable>();

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
        spriteRenderer.DOFade(fade, duration: 1.5f).OnComplete(() => {
            interactable.enabled = true;
        });

        changeShineColor.SetColor(scriptableCard.Rarity);
    }


    private void SetShowCardInfo(bool show) {
        if (show) {
            ChestItemInfoUI.Instance.SetCardInfo(scriptableCard);
        }
        else {
            ChestItemInfoUI.Instance.RemoveInfo();
        }
    }

    protected virtual void OnInteract() {
        interactable.enabled = false;

        transform.DOScale(Vector2.zero, duration: 0.2f).SetEase(Ease.InSine).OnComplete(() => {

            ResourceSystem.Instance.UnlockCard(scriptableCard.CardType);
            FeedbackPlayerOld.Play("NewCardUnlocked");
            NewCardUnlockedPanel.Instance.Setup(scriptableCard);

            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.UnlockCard);

            transform.DOKill();
            gameObject.ReturnToPool();
        });
    }
}
