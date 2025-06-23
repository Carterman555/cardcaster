using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class EnchantmentInfo : MonoBehaviour {

    [SerializeField] private Image enchantmentImage;
    [SerializeField] private TextMeshProUGUI enchantmentName;
    [SerializeField] private TextMeshProUGUI enchantmentDescription;

    private ScriptableEnchantment scriptableEnchantment;

    private void OnEnable() {
        LocalizationSettings.Instance.OnSelectedLocaleChanged += UpdateText;
    }

    private void OnDisable() {
        LocalizationSettings.Instance.OnSelectedLocaleChanged -= UpdateText;
    }

    public void Setup(EnchantmentType enchantmentType) {

        scriptableEnchantment = ResourceSystem.Instance.GetEnchantment(enchantmentType);

        enchantmentImage.sprite = scriptableEnchantment.Sprite;
        UpdateText(null);
    }

    private void UpdateText(Locale locale) {
        enchantmentName.text = scriptableEnchantment.Name;

        if (scriptableEnchantment.HasDescription) {
            enchantmentDescription.text = scriptableEnchantment.Name;
        }
        else {
            enchantmentDescription.text = StatsFormatter.Instance.GetStatModifiersStr(scriptableEnchantment.StatModifiers);
        }
    }

}
