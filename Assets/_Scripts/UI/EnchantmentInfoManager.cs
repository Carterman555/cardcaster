using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnchantmentInfoManager : StaticInstance<EnchantmentInfoManager> {

    [SerializeField] private EnchantmentInfo enchantmentInfo;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float minXPos = float.NegativeInfinity;
    [SerializeField] private float maxYPos = float.PositiveInfinity;
    private Tween scalingTween;

    public void ShowEnchantmentInfo(EnchantmentType enchantmentType, Vector2 position) {
        StopAllCoroutines();
        StartCoroutine(ShowEnchantmentInfoCor(enchantmentType, position));
    }
    private IEnumerator ShowEnchantmentInfoCor(EnchantmentType enchantmentType, Vector2 position) {
        while (scalingTween != null && scalingTween.active && scalingTween.IsPlaying()) {
            yield return null;
        }

        enchantmentInfo.Setup(enchantmentType);

        enchantmentInfo.gameObject.SetActive(true);

        enchantmentInfo.transform.position = position + offset;

        // so doesn't go off screen
        Vector2 infoPos = enchantmentInfo.GetComponent<RectTransform>().anchoredPosition;
        infoPos.x = Mathf.Clamp(infoPos.x, minXPos, maxYPos);
        enchantmentInfo.GetComponent<RectTransform>().anchoredPosition = infoPos;

        enchantmentInfo.transform.localScale = Vector3.zero;
        scalingTween = enchantmentInfo.transform.DOScale(1f, duration: 0.2f).SetUpdate(true);
    }

    public void HideEnchantmentInfo() {
        StopAllCoroutines();
        StartCoroutine(HideEnchantmentInfoCor());
    }
    private IEnumerator HideEnchantmentInfoCor() {
        while (scalingTween != null && scalingTween.active && scalingTween.IsPlaying()) {
            yield return null;
        }

        enchantmentInfo.transform.localScale = Vector3.one;
        scalingTween = enchantmentInfo.transform.DOScale(0f, duration: 0.2f).SetUpdate(true).OnComplete(() => {
            enchantmentInfo.gameObject.SetActive(false);
        });
    }
}
