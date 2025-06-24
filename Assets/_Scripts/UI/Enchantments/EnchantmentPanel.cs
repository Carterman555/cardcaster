using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnchantmentPanel : MonoBehaviour, IInitializable {

    public static EnchantmentPanel Instance { get; private set; }
    public void Initialize() {
        Instance = this;
    }
    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    [SerializeField] private Transform enchantmentImageContainer;
    [SerializeField] private EnchantmentImage enchantmentImagePrefab;

    private List<EnchantmentImage> enchantmentImages;

    private void OnEnable() {
        enchantmentImages = new();
        foreach (var enchantmentAmount in StatsManager.Enchantments) {
            EnchantmentImage enchantmentImage = enchantmentImagePrefab.Spawn(enchantmentImageContainer);
            enchantmentImage.Setup(enchantmentAmount.Key, enchantmentAmount.Value);
            enchantmentImages.Add(enchantmentImage);
        }
    }

    private void OnDisable() {
        foreach (EnchantmentImage enchantmentImage in enchantmentImages) {
            enchantmentImage.gameObject.ReturnToPool();
        }
        enchantmentImages.Clear();
    }

    [Header("Enchantment Info")]
    [SerializeField] private EnchantmentInfo enchantmentInfo;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float minXPos;
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
        infoPos.x = Mathf.Max(infoPos.x, minXPos);
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
