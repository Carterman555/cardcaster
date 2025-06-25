using System;
using System.Collections.Generic;
using UnityEngine;

public class CompendiumEnchantments : MonoBehaviour {
    [SerializeField] private Transform enchantmentImageContainer;
    [SerializeField] private EnchantmentImage enchantmentImagePrefab;

    private List<EnchantmentImage> enchantmentImages;

    private void OnEnable() {
        enchantmentImages = new();
        foreach (EnchantmentType enchantmentType in Enum.GetValues(typeof(EnchantmentType))) {
            EnchantmentImage enchantmentImage = enchantmentImagePrefab.Spawn(enchantmentImageContainer);
            enchantmentImage.Setup(enchantmentType, 1);
            enchantmentImages.Add(enchantmentImage);
        }
    }

    private void OnDisable() {
        foreach (EnchantmentImage enchantmentImage in enchantmentImages) {
            enchantmentImage.gameObject.ReturnToPool();
        }
        enchantmentImages.Clear();
    }
}
