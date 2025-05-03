using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ModifierImage : MonoBehaviour {

    [SerializeField] private Vector2 firstModifierPos;

    private ScriptableModifierCardBase modifier;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    private void OnEnable() {
        AbilityManager.OnApplyModifiers += Dissolve;

        HandCard.OnAnyStartPlaying_Card += CheckToShowX;
        HandCard.OnAnyCardUsed_Card += OnCardUsed;
        HandCard.OnAnyCancel_Card += TryShowModifierImage;
    }

    private void OnDisable() {
        AbilityManager.OnApplyModifiers -= Dissolve;

        HandCard.OnAnyStartPlaying_Card -= CheckToShowX;
        HandCard.OnAnyCardUsed_Card -= OnCardUsed;
        HandCard.OnAnyCancel_Card -= TryShowModifierImage;
    }

    public void Setup(ScriptableModifierCardBase modifier) {
        this.modifier = modifier;
        image.sprite = modifier.Sprite;

        MoveToModifierPosition();
    }

    private void MoveToModifierPosition() {

        GridLayoutGroup modifiersGrid = Containers.Instance.ActiveModifierImages.GetComponent<GridLayoutGroup>();
        
        int modifiersPerRow = modifiersGrid.constraintCount;
        int modifierNumber = AbilityManager.Instance.ActiveModifierCount();
        Vector2 gridPos = new(modifierNumber % modifiersPerRow, -(modifierNumber / modifiersPerRow));

        float spacing = modifiersGrid.cellSize.x + modifiersGrid.spacing.x;
        Vector2 targetPos = firstModifierPos + (gridPos * spacing);

        float moveDuration = 0.3f;
        GetComponent<RectTransform>().DOMove(targetPos, moveDuration).OnComplete(() => {
            transform.SetParent(Containers.Instance.ActiveModifierImages);
        });
    }

    #region Dissolve When Used

    [Header("Dissolve")]
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveSpeed;

    private DissolverVisual dissolverVisual;

    private void Dissolve() {
        dissolverVisual = new DissolverVisual(dissolveMaterial, new Image[] { image }, dissolveSpeed);
        dissolverVisual.AddDissolvedOutListener(OnDissolved);

        StartCoroutine(dissolverVisual.DissolveOut(true));
    }

    private void OnDissolved() {
        gameObject.ReturnToPool();
    }

    #endregion

    #region InCompatible With Ability Feedback

    [Header("Red X")]
    [SerializeField] private Sprite xSprite;

    // if an ability card that is not compatible with the modifier this image is showing, show a red x
    private void CheckToShowX(ScriptableCardBase card) {
        if (card is ScriptableAbilityCardBase ability) {
            if (!ability.IsCompatibleWithModifier(modifier)) {

                // switch to red x
                float scaleDuration = 0.15f;

                transform.DOScale(0, scaleDuration).OnComplete(() => {
                    image.sprite = xSprite;
                    transform.DOScale(1, scaleDuration);
                });
            }
        }
    }

    // switch this image back to modifier image if a nonmodifiable ability card is used because the card doesn't use
    // the modifier, but is still shows a red x when trying to play
    private void OnCardUsed(ScriptableCardBase card) {
        if (card is ScriptableAbilityCardBase ability) {
            if (!ability.IsModifiable) {
                SwitchToModifierImage();
            }
        }
    }

    private void TryShowModifierImage(ScriptableCardBase card) {
        if (card is ScriptableAbilityCardBase ability) {
            if (!ability.IsCompatibleWithModifier(modifier)) {
                SwitchToModifierImage();
            }
        }
    }

    private void SwitchToModifierImage() {
        float scaleDuration = 0.15f;
        transform.DOScale(0, scaleDuration).OnComplete(() => {
            image.sprite = modifier.Sprite;
            transform.DOScale(1, scaleDuration);
        });
    }

    #endregion
}
