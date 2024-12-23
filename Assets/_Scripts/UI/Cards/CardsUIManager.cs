using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [SerializeField] private HandCard cardButtonPrefab;
    private List<HandCard> cardButtons = new();

    private void OnEnable() {
        HandCard.OnAnyCardUsed_ButtonAndCard += OnCardUsed;

        DeckManager.OnGainCardToHand += DrawCardToEnd;
        DeckManager.OnTrashCardInHand += UpdateCardButtons;
        DeckManager.OnReplaceCardInHand += UpdateCardButtons;

        DrawCardsOnNewLevel();
    }
    private void OnDisable() {
        HandCard.OnAnyCardUsed_ButtonAndCard -= OnCardUsed;

        DeckManager.OnGainCardToHand -= DrawCardToEnd;
        DeckManager.OnTrashCardInHand -= UpdateCardButtons;
        DeckManager.OnReplaceCardInHand -= UpdateCardButtons;
    }

    private void DrawCardsOnNewLevel() {

        int handSize = DeckManager.Instance.GetHandSize();
        for (int i = 0; i < handSize; i++) {
            DrawCardToEnd();
        }

    }

    private void DrawCardToEnd() {
        int index = cardButtons.Count;
        DrawCard(index);

        UpdateCardButtons();
    }

    private void OnCardUsed(HandCard cardButton, ScriptableCardBase cardUsed) {

        // return the card button
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

    // spawn in a new card and set it up
    private void DrawCard(int index) {
        print("drawCard");

        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        ScriptableCardBase card = cardsInHand[index];
        HandCard cardButton = cardButtonPrefab.Spawn(transform);

        cardButton.Setup(deckButtonTransform, card);

        // add to list at correct index
        cardButtons.Insert(index, cardButton);

        UpdateCardPositions();

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.DrawCard);
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
            cardButtons[cardButtonIndex].SetCardPosition(pos);
        }
    }
}
