using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : GameButton {

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI hotkeyText;

    [SerializeField] private Image[] essenceImages;

    private ScriptableCardBase card;
    private int cardIndex;

    public void Setup(ScriptableCardBase card, int cardIndex) {
        this.cardIndex = cardIndex;
        SetCard(card);
    }

    private void Update() {
        bool canAfford = DeckManager.Instance.GetEssence() >= card.GetCost();
        button.interactable = canAfford;

        if (Input.GetKeyDown(KeyCode.Alpha1) && cardIndex == 0 ||
            Input.GetKeyDown(KeyCode.Alpha2) && cardIndex == 1 ||
            Input.GetKeyDown(KeyCode.Alpha3) && cardIndex == 2) {
            OnClicked();
        }
    }

    protected override void OnClicked() {
        base.OnClicked();

        card.Play();

        DeckManager.Instance.UseCard(cardIndex);
    }

    public void SetCard(ScriptableCardBase card) {
        this.card = card;

        titleText.text = card.GetName();
        hotkeyText.text = (cardIndex + 1).ToString();


        for (int i = 0; i < card.GetCost(); i++) {
            essenceImages[i].enabled = true;
        }
        for (int i = card.GetCost(); i < essenceImages.Length; i++) {
            essenceImages[i].enabled = false;
        }
    }
}
