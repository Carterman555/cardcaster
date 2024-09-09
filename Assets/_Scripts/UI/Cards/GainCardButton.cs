using TMPro;
using UnityEngine;

public class GainCardButton : GameButton {

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;

    private ScriptableCardBase card;

    public void Setup(ScriptableCardBase card) {
        this.card = card;

        titleText.text = card.GetName();
        descriptionText.text = card.GetDescription();
        costText.text = card.GetCost().ToString();
    }

    private void Update() {
        button.interactable = !ChooseCardPanel.Instance.ChoseCard();
    }

    protected override void OnClick() {
        base.OnClick();

        DeckManager.Instance.GainCard(card);
        ChooseCardPanel.Instance.SetChoseCard();

        //PopupCanvas.Instance.DeactivateUIObject("ChooseCardPanel");

        Time.timeScale = 1;
    }
}
