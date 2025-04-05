using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorFromRarity : MonoBehaviour {

    [SerializeField] private bool setSpriteRenderer;
    [ConditionalHide("setSpriteRenderer")] [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private bool setImage;
    [ConditionalHide("setImage")][SerializeField] private Image image;

    [SerializeField] private bool setTextUI;
    [ConditionalHide("setTextUI")][SerializeField] private TextMeshProUGUI textUI;

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor;
    [SerializeField] private Color uncommonColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color mythicColor;

    public void SetColor(Rarity rarity) {

        Color color = commonColor;

        if (rarity == Rarity.Common) {
            color = commonColor;
        }
        else if (rarity == Rarity.Uncommon) {
            color = uncommonColor;
        }
        else if (rarity == Rarity.Rare) {
            color = rareColor;
        }
        else if (rarity == Rarity.Epic) {
            color = epicColor;
        }
        else if (rarity == Rarity.Mythic) {
            color = mythicColor;
        }

        if (setSpriteRenderer) {
            spriteRenderer.color = color;
        }
        
        if (setImage) {
            image.color = color;
        }

        if (setTextUI) {
            textUI.color = color;
        }
    }
}
