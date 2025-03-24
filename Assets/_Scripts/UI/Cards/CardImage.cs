using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardImage : MonoBehaviour {

    [SerializeField] private Image cardTypeImage;
    [SerializeField] private Color abilityTypeColor;
    [SerializeField] private Color modifierTypeColor;
    [SerializeField] private Color persisentTypeColor;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] essenceImages;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private UpgradeSlots upgradeSlots;

    public void Setup(ScriptableCardBase card) {

        if (card is ScriptableAbilityCardBase) {
            upgradeSlots.gameObject.SetActive(false);

            cardTypeImage.color = abilityTypeColor;
            typeText.text = "Ability";
        }
        else if (card is ScriptableModifierCardBase) {
            upgradeSlots.gameObject.SetActive(false);

            cardTypeImage.color = modifierTypeColor;
            typeText.text = "Modifier";
        }
        else if (card is ScriptablePersistentCard persistentCard) {
            upgradeSlots.gameObject.SetActive(true);
            upgradeSlots.Setup(persistentCard);

            cardTypeImage.color = persisentTypeColor;
            typeText.text = "Persistent";
        }

        iconImage.sprite = card.Sprite;

        SetupCostImages(card.Cost);
        titleText.text = card.Name;
        descriptionText.text = card.Description;

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
}
