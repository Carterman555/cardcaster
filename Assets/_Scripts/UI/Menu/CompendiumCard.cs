using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CompendiumCard : MonoBehaviour {

    [SerializeField] private CardImage cardImage;

    [SerializeField] private Image iconImage;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private UpgradeSlots upgradeSlots;

    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private LocalizedString locLockedStr;

    public void Setup(CardType cardType, bool locked) {

        cardImage.Setup(ResourceSystem.Instance.GetCard(cardType));

        if (locked) {
            cardImage.GetComponent<CanvasGroup>().alpha = 0.65f;

            iconImage.sprite = lockedIcon;
            titleText.text = locLockedStr.GetLocalizedString();
            descriptionText.text = "";

            cardImage.GetComponent<ChangeColorFromRarity>().SetColor(Rarity.Common);
            upgradeSlots.gameObject.SetActive(false);
        }
        else {
            cardImage.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }
}
