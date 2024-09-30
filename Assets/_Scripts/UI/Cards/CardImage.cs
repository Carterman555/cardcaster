using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardImage : MonoBehaviour {

    private Image cardImage;
    [SerializeField] private Sprite abilityCardFront;
    [SerializeField] private Sprite modifierCardFront;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] essenceImages;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake() {
        cardImage = GetComponent<Image>();
    }

    public void Setup(ScriptableCardBase card) {

        if (card is ScriptableAbilityCardBase) {
            cardImage.sprite = abilityCardFront;
        }
        else if (card is ScriptableModifierCardBase) {
            cardImage.sprite = modifierCardFront;
        }

        iconImage.sprite = card.GetSprite();
        SetupCostImages(card.GetCost());
        titleText.text = card.GetName();
        descriptionText.text = card.GetDescription();
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
