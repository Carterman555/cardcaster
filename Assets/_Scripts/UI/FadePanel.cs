using DG.Tweening;
using UnityEngine.UI;

public class FadePanel : StaticInstance<FadePanel> {

    private Image image;

    protected override void Awake() {
        base.Awake();
        image = GetComponent<Image>();
    }

    private float duration = 1.5f;

    public void FadeIn() {
        image.enabled = true;
        image.Fade(1);
        image.DOFade(0, duration).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() => {
            image.enabled = false;
        });
    }

    public void FadeOut() {
        image.enabled = true;
        image.Fade(0);
        image.DOFade(1, duration).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void StayFadedOut() {
        image.enabled = true;
        image.Fade(1);
    }
}