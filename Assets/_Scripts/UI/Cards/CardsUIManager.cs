using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [FormerlySerializedAs("cardButtonPrefab")]
    [SerializeField] private HandCard handCardPrefab;
    private List<HandCard> handCards = new();

    private void OnEnable() {
        DeckManager.OnGainCardToHand += DrawCardToEnd;
        DeckManager.OnTrashCardInHand += OnTrashCard;
        DeckManager.OnReplaceCardInHand += UpdateCardButtons;
        DeckManager.OnClearCards += OnClearCards;
        DeckManager.OnHandSizeChanged_Size += OnHandSizeChanged;

        DrawCardsOnNewLevel();
    }

    private void OnDisable() {
        DeckManager.OnGainCardToHand -= DrawCardToEnd;
        DeckManager.OnTrashCardInHand -= OnTrashCard;
        DeckManager.OnReplaceCardInHand -= UpdateCardButtons;
        DeckManager.OnClearCards -= OnClearCards;
        DeckManager.OnHandSizeChanged_Size -= OnHandSizeChanged;
    }

    private void DrawCardsOnNewLevel() {

        int handSize = DeckManager.Instance.GetHandSize();
        for (int i = 0; i < handSize; i++) {
            DrawCardToEnd();
        }
    }

    private void DrawCardToEnd() {
        int index = handCards.Count;
        DrawCard(index);

        UpdateCardButtons();
    }

    private void OnTrashCard(bool usingCard) {
        // if using the card (persistent trash when using), then don't update the card buttons because TryReplaceUsedCard and
        // TrySpawnCardsToEnd still need to update the amount of card buttons. And using the card will invoke UpdateCardButtons
        if (!usingCard) {
            UpdateCardButtons();
        }
    }

    private void OnClearCards() {
        foreach (HandCard handCard in handCards) {
            handCard.gameObject.ReturnToPool();
        }
        handCards.Clear();
    }

    private void OnHandSizeChanged(int handSize) {

        while (handSize < handCards.Count) {
            handCards[^1].gameObject.ReturnToPool();
            handCards.RemoveAt(handCards.Count - 1);
        }

        bool canDrawCard = DeckManager.Instance.GetCardsInHand()[handCards.Count] != null;
        while (handSize > handCards.Count && canDrawCard) {
            int index = handCards.Count;
            DrawCard(index);

            canDrawCard = DeckManager.Instance.GetCardsInHand()[handCards.Count] != null;
        }

        UpdateCardButtons();
    }

    public void ReturnUsedCard(HandCard handCard) {
        // return the card button
        handCard.gameObject.ReturnToPool();
        handCards.Remove(handCard);
    }

    public void TryReplaceUsedCard(HandCard handCard) {
        // respawn the card button if it was replaced (most likely will)
        if (GetHandSize() > handCards.Count) {
            DrawCard(handCard.GetIndex());
        }
    }

    public void TrySpawnCardsToEnd() {
        // spawn more cards to end. This happens when modifier cards are used on an ability and so they all
        // become available to be drawn to the deck
        while (GetHandSize() > handCards.Count) {
            DrawCardToEnd();
        }
    }

    // spawn in a new card and set it up
    private void DrawCard(int index) {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        ScriptableCardBase card = cardsInHand[index];
        HandCard cardButton = handCardPrefab.Spawn(transform);
        cardButton.Setup(deckButtonTransform, card);

        // add to list at correct index
        handCards.Insert(index, cardButton);

        UpdateCardPositions();

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.DrawCard, uiSound: true);
    }

    public void UpdateCardButtons() {
        UpdateScriptableCards();
        UpdateCardIndexes();
        UpdateCardPositions();
    }

    private void UpdateScriptableCards() {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        for (int i = 0; i < handCards.Count; i++) {
            if (cardsInHand[i] != null) {
                handCards[i].SetCard(cardsInHand[i]);
            }
            else {
                handCards[i].gameObject.ReturnToPool();
                handCards.RemoveAt(i);
            }
        }
    }

    private void UpdateCardIndexes() {
        for (int i = 0; i < handCards.Count; i++) {
            handCards[i].SetCardIndex(i);
        }
    }

    private int GetHandSize() {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();
        int handSize = cardsInHand.Count(card => card != null);
        return handSize;
    }

    [Header("Card Positioning")]
    [SerializeField] private RectTransform deckButtonTransform;
    [SerializeField] private float cardYPos;
    [SerializeField] private float cardSpacing;

    // this method returns the pos to center the cards in the bottom center
    private Vector2 GetCardPos(int index, int handSize, int maxHandSize) {
        float firstCardXPos = -((handSize - 1) * cardSpacing * 0.5f);
        float thisCardXPos = firstCardXPos + (cardSpacing * index);
        return new Vector3(thisCardXPos, cardYPos);
    }

    private void UpdateCardPositions() {
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand();

        for (int cardButtonIndex = 0; cardButtonIndex < handCards.Count; cardButtonIndex++) {
            Vector2 pos = GetCardPos(cardButtonIndex, GetHandSize(), cardsInHand.Length);
            handCards[cardButtonIndex].SetCardPosition(pos);
        }
    }
}
