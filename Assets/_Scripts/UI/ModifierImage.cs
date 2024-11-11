using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ModifierImage : MonoBehaviour {

    [SerializeField] private Vector2 firstModifierPos;
    [SerializeField] private float modifierSpacing;

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
        image.sprite = modifier.GetSprite();

        MoveToModifierPosition();
    }

    private void MoveToModifierPosition() {

        Vector2 targetPosition = firstModifierPos;
        targetPosition += new Vector2(AbilityManager.Instance.ActiveModifierCount() * modifierSpacing, 0f);

        float moveDuration = 0.3f;
        transform.DOMove(targetPosition, moveDuration).OnComplete(() => {
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
            if (!ability.IsCompatible(modifier)) {
                SwitchToRedX();
            }
        }
    }

    private void SwitchToRedX() {
        float scaleDuration = 0.15f;

        transform.DOKill();
        transform.DOScale(0, scaleDuration).OnComplete(() => {
            image.sprite = xSprite;
            transform.DOScale(1, scaleDuration);
        });
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
            if (!ability.IsCompatible(modifier)) {
                SwitchToModifierImage();
            }
        }
    }

    private void SwitchToModifierImage() {
        float scaleDuration = 0.15f;

        transform.DOKill();
        transform.DOScale(0, scaleDuration).OnComplete(() => {
            image.sprite = modifier.GetSprite();
            transform.DOScale(1, scaleDuration);
        });
    }

    #endregion
}
