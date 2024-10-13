using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [SerializeField] private CardButton cardButtonPrefab;
    private List<CardButton> cardButtons = new();

    public void DrawCard(int index) {
        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        UpdateCardButtonsCount();

        // cardsInHand[index] is null when a modifier card is played and the deck doesn't have any more cards to
        // replace the modifier card that was in the hand
        if (cardsInHand[index] != null) {
            cardButtons[index].OnDrawCard(cardsInHand[index]);
        }
        else {
            UpdateCardPositions();
        }
    }

    private void OnEnable() {
        CardButton.OnAnyCardPlayed += UpdateCardButtonsCount;
    }
    private void OnDisable() {
        CardButton.OnAnyCardPlayed -= UpdateCardButtonsCount;
    }

    private void UpdateCardButtons() {
        UpdateCardButtonsCount();

        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        for (int i = 0; i < cardButtons.Count; i++) {
            cardButtons[i].SetCard(cardsInHand[i]);
        }

        UpdateCardPositions();
    }

    // make sure the number of card button should always be eqaul to the number of cards in the hand
    private void UpdateCardButtonsCount() {
        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();
        int handSize = cardsInHand.Where(card => card != null).Count();

        // add card buttons until they match the number of cards in hand
        while (cardButtons.Count < handSize) {
            CardButton newCardButton = SpawnNewCard();
        }

        // remove card buttons until they match the number of cards in hand
        while (cardButtons.Count > handSize) {
            cardButtons.Last().gameObject.ReturnToPool();
            cardButtons.RemoveAt(cardButtons.Count - 1);
        }
    }

    public CardButton SpawnNewCard() {

        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        int index = cardButtons.Count;

        ScriptableCardBase card = cardsInHand[index];
        CardButton cardButton = cardButtonPrefab.Spawn(transform);

        int handSize = cardsInHand.Where(card => card != null).Count();
        Vector2 pos = GetCardPos(index, handSize, cardsInHand.Count);

        cardButton.Setup(index, deckButtonTransform, pos);

        cardButtons.Add(cardButton);

        return cardButton;
    }

    [Header("Card Positioning")]
    [SerializeField] private RectTransform deckButtonTransform;
    [SerializeField] private float cardYPos;
    [SerializeField] private float cardSpacing;

    // this method returns the pos to center the cards in the bottom center
    private Vector2 GetCardPos(int index, int handSize, int maxHandSize) {

        float screenXCenter = Screen.width / 2;

        float firstCardXPos = screenXCenter - ((handSize - 1) * cardSpacing * 0.5f);
        float thisCardXPos = firstCardXPos + (cardSpacing * index);
        return new Vector3(thisCardXPos, cardYPos);
    }

    private void UpdateCardPositions(CardButton cardButtonToNotUpdate = null) {
        List<ScriptableCardBase> cardsInHand = DeckManager.Instance.GetCardsInHand();

        for (int cardButtonIndex = 0; cardButtonIndex < cardButtons.Count; cardButtonIndex++) {

            if (cardButtons[cardButtonIndex] == cardButtonToNotUpdate) {
                continue;
            }

            int handSize = cardsInHand.Where(card => card != null).Count();
            Vector2 pos = GetCardPos(cardButtonIndex, handSize, cardsInHand.Count);
            cardButtons[cardButtonIndex].SetCardPosition(pos, true);
        }
    }
}
