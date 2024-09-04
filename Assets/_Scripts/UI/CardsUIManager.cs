using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [SerializeField] private CardButton cardButtonPrefab;
    private List<CardButton> cardButtons = new();

    [SerializeField] private Image essenceFill;

    public void Setup() {

        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();
        for (int i = 0; i < cardsInHand.Count; i++) {
            ScriptableCardBase card = cardsInHand[i];
            CardButton cardButton = cardButtonPrefab.Spawn(transform);
            cardButton.Setup(card, i);

            cardButtons.Add(cardButton);
        }
    }

    private void Update() {
        essenceFill.fillAmount = DeckManager.Instance.GetEssenceFraction();
    }

    public void ReplaceCard(int index) {
        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        cardButtons[index].SetCard(cardsInHand[index]);
    }
}
