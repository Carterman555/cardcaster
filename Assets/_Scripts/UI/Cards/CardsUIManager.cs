using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [SerializeField] private CardButton cardButtonPrefab;
    private List<CardButton> cardButtons = new();

    [Header("Card Positioning")]
    [SerializeField] private RectTransform deckButtonTransform;
    [SerializeField] private float cardYPos;
    [SerializeField] private float cardSpacing;

    public void Setup() {

        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        float cardXPos = -((cardsInHand.Count - 1) * cardSpacing * 0.5f);

        for (int i = 0; i < cardsInHand.Count; i++) {
            ScriptableCardBase card = cardsInHand[i];
            CardButton cardButton = cardButtonPrefab.Spawn(transform);
            cardButton.Setup(i, deckButtonTransform, new Vector3(cardXPos, cardYPos));
            cardButtons[i].OnDrawCard(cardsInHand[i]);

            cardXPos += cardSpacing;

            cardButtons.Add(cardButton);
        }
    }

    public void DrawCard(int index) {
        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        // it's null when the deck runs out of cards in deck
        if (cardsInHand[index] != null) {
            cardButtons[index].OnDrawCard(cardsInHand[index]);
        }
    }

    public void SpawnNewCard(int index) {

        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        ScriptableCardBase card = cardsInHand[index];
        CardButton cardButton = cardButtonPrefab.Spawn(transform);
        cardButton.Setup(index, deckButtonTransform, new Vector3(cardXPos, cardYPos));
        cardButtons[index].OnDrawCard(cardsInHand[index]);

        cardButtons.Add(cardButton);
    }
}
