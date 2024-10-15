using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [SerializeField] private CardButton cardButtonPrefab;
    private List<CardButton> cardButtons = new();

    private void OnEnable() {
        //DeckManager.OnHandChanged += UpdateCardButtons;
        DeckManager.OnGainCardToHand += DrawCardToEnd;
        CardButton.OnAnyCardUsed_ButtonAndCard += OnCardUsed;
    }
    private void OnDisable() {
        //DeckManager.OnHandChanged -= UpdateCardButtons;
        DeckManager.OnGainCardToHand -= DrawCardToEnd;
        CardButton.OnAnyCardUsed_ButtonAndCard -= OnCardUsed;
    }

    private void DrawCardToEnd() {
        int index = cardButtons.Count;
        DrawCard(index);

        UpdateCardButtons();
    }

    private void OnCardUsed(CardButton cardButton, ScriptableCardBase cardUsed) {

        // get rid of the card button
        cardButton.gameObject.ReturnToPool();
        cardButtons.Remove(cardButton);

        // respawn the card button if it was replaced (most likely will)
        if (GetHandSize() > cardButtons.Count) {
            DrawCard(cardButton.GetIndex());
        }

        // spawn more cards to end. This happens when modifier cards are used on an ability and so they all
        // become available to be drawn to the deck
        while (GetHandSize() > cardButtons.Count) {
            DrawCardToEnd();
        }

        UpdateCardButtons();
    }

    private void DrawCard(int index) {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        CardButton newCardButton = SpawnNewCard(index);
        ScriptableCardBase newCard = cardsInHand[index];

        UpdateCardPositions();

        newCardButton.OnDrawCard(newCard);
    }

    public CardButton SpawnNewCard(int index) {

        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        ScriptableCardBase card = cardsInHand[index];
        CardButton cardButton = cardButtonPrefab.Spawn(transform);

        Vector2 pos = GetCardPos(index, GetHandSize(), cardsInHand.Length);

        cardButton.Setup(index, deckButtonTransform, pos);

        // add to list at correct index
        cardButtons.Insert(index, cardButton);

        return cardButton;
    }

    private void UpdateCardButtons() {
        UpdateScriptableCards();
        UpdateCardIndexes();
        UpdateCardPositions();
    }

    private void UpdateScriptableCards() {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        for (int i = 0; i < cardButtons.Count; i++) {
            cardButtons[i].SetCard(cardsInHand[i]);
        }
    }

    private void UpdateCardIndexes() {
        for (int i = 0; i < cardButtons.Count; i++) {
            cardButtons[i].SetCardIndex(i);
        }
    }

    private int GetHandSize() {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();
        int handSize = cardsInHand.Where(card => card != null).Count();
        return handSize;
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

    private void UpdateCardPositions() {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        for (int cardButtonIndex = 0; cardButtonIndex < cardButtons.Count; cardButtonIndex++) {

            Vector2 pos = GetCardPos(cardButtonIndex, GetHandSize(), cardsInHand.Length);

            bool moveCard = !cardButtons[cardButtonIndex].IsPlayingCard();
            cardButtons[cardButtonIndex].SetCardPosition(pos, moveCard);
        }
    }
}
