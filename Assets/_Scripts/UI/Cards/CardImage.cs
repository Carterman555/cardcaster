using TMPro;
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

        if (card == null) {
            Debug.LogError("Trying to setup card image with null card!");
            return;
        }

        this.card = card;

        if (card is ScriptableAbilityCardBase || card is ScriptableBlankMemoryCard) {
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
        UpdateTexts(LocalizationSettings.SelectedLocale);

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
        LocalizationSettings.SelectedLocaleChanged += UpdateTexts;
        UpdateTexts(LocalizationSettings.SelectedLocale); // needed here and setup because well I don't want to explain
    }
    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateTexts;
    }

    private void UpdateTexts(Locale locale) {
        if (card == null) {
            return;
        }

        titleText.text = card.Name.GetLocalizedString();
        catagoryText.text = card.LocCategory.GetLocalizedString();
        descriptionText.text = card.Description.GetLocalizedString();
    }
}
