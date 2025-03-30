using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CardImage : MonoBehaviour {

    [SerializeField] private Image cardTypeImage;
    [SerializeField] private Color abilityTypeColor;
    [SerializeField] private Color modifierTypeColor;
    [SerializeField] private Color persisentTypeColor;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] essenceImages;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI catagoryText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private ScriptableCardBase card;

    [SerializeField] private UpgradeSlots upgradeSlots;

    public void Setup(ScriptableCardBase card) {
        this.card = card;

        if (card is ScriptableAbilityCardBase) {
            upgradeSlots.gameObject.SetActive(false);

            cardTypeImage.color = abilityTypeColor;
        }
        else if (card is ScriptableModifierCardBase) {
            upgradeSlots.gameObject.SetActive(false);

            cardTypeImage.color = modifierTypeColor;
        }
        else if (card is ScriptablePersistentCard persistentCard) {
            upgradeSlots.gameObject.SetActive(true);
            upgradeSlots.Setup(persistentCard);

            cardTypeImage.color = persisentTypeColor;
        }

        iconImage.sprite = card.Sprite;

        SetupCostImages(card.Cost);
        titleText.text = card.Name.GetLocalizedString();
        catagoryText.text = card.Category.GetLocalizedString();
        descriptionText.text = card.Description.GetLocalizedString();

        GetComponent<ChangeColorFromRarity>().SetColor(card.Rarity);
    }

    private void SetupCostImages(int cost) {
        for (int i = 0; i < cost; i++) {
            essenceImages[i].enabled = true;
        }
        for (int i = cost; i < essenceImages.Length; i++) {
            essenceImages[i].enabled = false;
        }
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }
    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(Locale locale) {
        titleText.text = card.Name.GetLocalizedString();
        catagoryText.text = card.Category.GetLocalizedString();
        descriptionText.text = card.Description.GetLocalizedString();
    }
}
