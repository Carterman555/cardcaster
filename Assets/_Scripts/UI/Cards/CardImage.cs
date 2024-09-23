using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardImage : MonoBehaviour {

    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] essenceImages;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Setup(ScriptableCardBase card) {
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
