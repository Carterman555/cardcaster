using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardImage : MonoBehaviour {

    private Image cardImage;
    [SerializeField] private Sprite abilityCardFront;
    [SerializeField] private Sprite modifierCardFront;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] essenceImages;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Setup(ScriptableCardBase card) {

        cardImage = GetComponent<Image>();
        if (card is ScriptableAbilityCardBase) {
            cardImage.sprite = abilityCardFront;
            typeText.text = "Ability";
        }
        else if (card is ScriptableModifierCardBase) {
            cardImage.sprite = modifierCardFront;
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
