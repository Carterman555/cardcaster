using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnchantmentPanel : MonoBehaviour {

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

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            if (enchantmentImages.Count > 0) {
                //... wait to give EnchantmentImageManager time to initialize and 
                //... enchantments panel to slide in

                float slideTime = 0.35f;
                DOVirtual.DelayedCall(slideTime, () => {
                    enchantmentImages[0].Select();
                });
            }
        }
        
    }

    private void OnDisable() {
        foreach (EnchantmentImage enchantmentImage in enchantmentImages) {
            enchantmentImage.gameObject.ReturnToPool();
        }
        enchantmentImages.Clear();
    }
}
