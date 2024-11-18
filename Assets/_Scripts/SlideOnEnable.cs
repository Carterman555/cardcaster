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

    private void OnEnable() {
        RectTransform rectTransform = GetComponent<RectTransform>();

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
}
