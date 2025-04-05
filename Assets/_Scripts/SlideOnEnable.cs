using DG.Tweening;
using UnityEngine;

public class SlideOnEnable : MonoBehaviour {

    [SerializeField] private bool defaultPosIsStart;

    [ConditionalHideReversed("defaultPosIsStart")][SerializeField] private Vector2 startPos;
    [ConditionalHide("defaultPosIsStart")][SerializeField] private Vector2 targetPos;

    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.InSine;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();

        if (defaultPosIsStart) {
            startPos = rectTransform.anchoredPosition;
        }
        else {
            targetPos = rectTransform.anchoredPosition;
        }
    }

    private void OnEnable() {
        Slide();
    }

    public void Slide() {
        rectTransform.anchoredPosition = startPos;
        DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, targetPos, duration).SetEase(ease).SetUpdate(true);
    }

    [Header("Slide Back")]
    [SerializeField] private bool differentSlideBackEase = true;
    [ConditionalHide("differentSlideBackEase")] [SerializeField] private Ease slideBackEase = Ease.OutSine;

    public void SlideBack() {
        Ease currentEase = differentSlideBackEase ? slideBackEase : ease;

        rectTransform.anchoredPosition = targetPos;
        DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, startPos, duration).SetEase(ease).SetUpdate(true);
    }
}
