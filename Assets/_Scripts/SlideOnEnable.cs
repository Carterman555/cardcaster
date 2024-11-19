using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SlideOnEnable : MonoBehaviour {

    [SerializeField] private bool currentPosIsStart;

    [ConditionalHideReversed("currentPosIsStart")][SerializeField] private Vector2 startPos;
    [ConditionalHide("currentPosIsStart")][SerializeField] private Vector2 targetPos;

    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.InSine;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        if (currentPosIsStart) {
            DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, targetPos, duration).SetEase(ease);

            //rectTransform.anchoredPosition =
        }
        else {
            targetPos = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = startPos;
            DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, targetPos, duration).SetEase(ease);
        }
    }

    [Header("Slide Back")]
    [SerializeField] private bool differentSlideBackEase = true;
    [ConditionalHide("differentSlideBackEase")] [SerializeField] private Ease slideBackEase = Ease.OutSine;

    public void SlideBack() {
        print("slide back");

        Ease currentEase = differentSlideBackEase ? slideBackEase : ease;

        DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, startPos, duration).SetEase(ease);
    }
}
