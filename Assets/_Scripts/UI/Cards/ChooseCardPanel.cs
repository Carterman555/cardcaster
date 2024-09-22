using TMPro;
using UnityEngine;

public class ChooseCardPanel : StaticInstance<ChooseCardPanel> {

    [SerializeField] private GainCardButton[] cardButtons;
    [SerializeField] private TextMeshProUGUI chooseText;

    private bool choseCard = false;

    public void Setup(ScriptableCardBaseOld[] cards) {

        chooseText.text = "Choose a Card!";

        for (int i = 0; i < cardButtons.Length; i++) {
            cardButtons[i].Setup(cards[i]);
        }

        choseCard = false;
    }

    public void SetChoseCard() {
        choseCard = true;
    }

    public bool ChoseCard() {
        return choseCard;
    }
}
