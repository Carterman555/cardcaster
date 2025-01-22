using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardImage : MonoBehaviour {

    [SerializeField] private Image cardTypeImage;
    [SerializeField] private Color abilityTypeColor;
    [SerializeField] private Color modifierTypeColor;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] essenceImages;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Setup(ScriptableCardBase card) {

        if (card is ScriptableAbilityCardBase) {
            cardTypeImage.color = abilityTypeColor;
            typeText.text = "Ability";
        }
        else if (card is ScriptableModifierCardBase) {
            cardTypeImage.color = modifierTypeColor;
            typeText.text = "Modifier";
        }

        iconImage.sprite = card.GetSprite();

        SetupCostImages(card.GetCost());
        titleText.text = card.GetName();
        descriptionText.text = card.GetDescription();

        GetComponent<ChangeColorFromRarity>().SetColor(card.GetRarity());
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
