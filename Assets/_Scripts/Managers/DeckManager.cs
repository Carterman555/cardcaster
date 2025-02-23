using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public class DeckManager : Singleton<DeckManager> {

    // events for the cardUIManager to update the card UI
    public static event Action OnGainCardToHand;
    public static event Action OnTrashCardInHand;
    public static event Action OnReplaceCardInHand;
    public static event Action OnClearCards;

    public static event Action<float> OnEssenceChanged_Amount;

    [SerializeField] private int startingCardAmount;
    [SerializeField] private int maxHandSize;

    private List<ScriptableCardBase> cardsInDeck = new();
    private List<ScriptableCardBase> cardsInDiscard = new();
    private ScriptableCardBase[] cardsInHand;
    private List<ScriptableCardBase> cardsInModifierStack = new();

    [SerializeField] private CardAmount[] startingCardPool;

    [SerializeField] private int maxEssence;
    private float essence;

    #region Get Methods

    public List<ScriptableCardBase> GetCardsInDeck() {
        return cardsInDeck;
    }

    public List<ScriptableCardBase> GetCardsInDiscard() {
        return cardsInDiscard;
    }

    public ScriptableCardBase[] GetCardsInHand() {
        return cardsInHand;
    }

    public int GetHandSize() {
        int handSize = cardsInHand.Where(card => card != null).Count();
        return handSize;
    }

    public List<ScriptableCardBase> GetAllCards() {
        List<ScriptableCardBase> allCards = new();
        allCards.AddRange(cardsInDeck);
        allCards.AddRange(cardsInDiscard);
        allCards.AddRange(cardsInHand);
        return allCards;
    }

    public float GetEssence() {
        return essence;
    }

    #endregion

    public void ChangeMaxEssence(int amount) {
        maxEssence += amount;
    }

    public void ChangeEssenceAmount(float amount) {

        if (DebugManager.Instance.UnlimitedEssence) { // for debugging
            return;
        }

        essence = Mathf.MoveTowards(essence, maxEssence, amount);

        OnEssenceChanged_Amount?.Invoke(essence);
    }

    private void OnEnable() {
        GameSceneManager.OnStartGame += OnStartGame;

        ClearDeckAndEssence();
    }
    private void OnDisable() {
        GameSceneManager.OnStartGame -= OnStartGame;
    }

    private void OnStartGame() {
        if (!GameSceneManager.Instance.Tutorial) {
            ClearDeckAndEssence();
            GiveStartingCards();
        }
    }

    public void ClearDeckAndEssence() {
        cardsInHand = new ScriptableCardBase[maxHandSize];

        cardsInDeck.Clear();
        cardsInDiscard.Clear();
        cardsInModifierStack.Clear();

        OnClearCards?.Invoke(); // clears handcards

        essence = maxEssence;
        OnEssenceChanged_Amount?.Invoke(essence);
    }

    private void GiveStartingCards() {

        //... availableCards has multiple copies of same cardtypes unlike startingCardPool which has the amounts as ints
        List<CardType> availableCards = new();

        foreach (CardAmount cardAmount in startingCardPool) {
            for (int i = 0; i < cardAmount.Amount; i++) {
                availableCards.Add(cardAmount.CardType);
            }
        }

        if (availableCards.Count < startingCardAmount) {
            Debug.LogWarning($"Not enough cards in pool! Available: {availableCards.Count}, Required: {startingCardAmount}");
            startingCardAmount = availableCards.Count;
        }


        // choose random cards from the pool and add them to the deck
        for (int i = 0; i < startingCardAmount; i++) {
            CardType choosenCardType = availableCards.RandomItem();
            availableCards.Remove(choosenCardType);
            GainCard(ResourceSystem.Instance.GetCardInstance(choosenCardType));
        }
    }

    public void OnUseAbilityCard(int indexInHand) {
        ChangeEssenceAmount(-cardsInHand[indexInHand].GetCost());
        DiscardCardInHand(indexInHand);
        OnUseCard(indexInHand);
    }

    public void OnUseModifierCard(int indexInHand) {
        ChangeEssenceAmount(-cardsInHand[indexInHand].GetCost());
        StackCardInHand(indexInHand);
        OnUseCard(indexInHand);
    }

    private void OnUseCard(int indexInHand) {

        // index in hand might be messed up - think about how it could be messed up

        TryDrawCard(indexInHand);
        TryDrawOtherCards();

        RemoveHandGaps();
    }

    #region Basic Deck Methods

    public void DiscardStackedCards() {
        cardsInDiscard.AddRange(cardsInModifierStack);
        cardsInModifierStack.Clear();
    }

    public void GainCard(ScriptableCardBase card) {

        if (GetHandSize() == maxHandSize) {
            cardsInDiscard.Add(card);
        }
        else {
            cardsInHand[GetHandSize()] = card;
            OnGainCardToHand?.Invoke();
        }


        // if gains a locked card, unlock it
        bool cardLocked = !ResourceSystem.Instance.GetUnlockedCards().Any(c => c == card.CardType);
        if (cardLocked && NewCardUnlockedPanel.Instance != null) {
            ResourceSystem.Instance.UnlockCard(card.CardType);
            FeedbackPlayerOld.Play("NewCardUnlocked");
            NewCardUnlockedPanel.Instance.Setup(card);
        }
    }

    public void TrashCard(CardLocation cardLocation, int cardIndex) {
        if (cardLocation == CardLocation.Deck) {
            cardsInDeck.RemoveAt(cardIndex);
        }
        else if (cardLocation == CardLocation.Discard) {
            cardsInDiscard.RemoveAt(cardIndex);
        }
        else if (cardLocation == CardLocation.Hand) {
            TryDrawCard(cardIndex);
            OnTrashCardInHand?.Invoke();
        }
    }

    public void ReplaceCard(CardLocation cardLocation, int cardIndex, ScriptableCardBase newCard) {
        if (cardLocation == CardLocation.Deck) {
            cardsInDeck[cardIndex] = newCard;
        }
        else if (cardLocation == CardLocation.Discard) {
            cardsInDiscard[cardIndex] = newCard;
        }
        else if (cardLocation == CardLocation.Hand) {
            cardsInHand[cardIndex] = newCard;
            OnReplaceCardInHand?.Invoke();
        }
    }

    private void DiscardCardInHand(int indexInHand) {
        cardsInDiscard.Add(cardsInHand[indexInHand]);
        cardsInHand[indexInHand] = null;
    }

    private void StackCardInHand(int indexInHand) {
        cardsInModifierStack.Add(cardsInHand[indexInHand]);
        cardsInHand[indexInHand] = null;
    }

    private void RemoveHandGaps() {

        // go through each card in hand and move the positions to remove gaps
        int writeIndex = 0;
        for (int readIndex = 0; readIndex < cardsInHand.Length; readIndex++) {
            if (cardsInHand[readIndex] != null) {
                cardsInHand[writeIndex] = cardsInHand[readIndex];
                writeIndex++;
            }
        }

        // set the rest of the cards in hand to null
        for (int i = writeIndex; i < cardsInHand.Length; i++) {
            cardsInHand[i] = null;
        }
    }

    private bool TryDrawCard(int indexInHand) {

        // ran out of cards to draw to hand so get cards from discard
        if (cardsInDeck.Count == 0) {
            ShuffleDiscardToDeck();

            // still out of cards to draw to hand, so don't
            if (cardsInDeck.Count == 0) {
                return false;
            }
        }

        cardsInHand[indexInHand] = cardsInDeck[0];
        cardsInDeck.RemoveAt(0);

        return true;
    }

    /// <summary>
    /// This method verifies if all the cards have been drawn into the player's hand. If the deck runs out of cards
    /// to draw, the cards that should still be drawn may end up beneath the hand. This method checks if there are
    /// any remaining cards to be drawn and attempts to draw them if necessary.
    /// </summary>
    private void TryDrawOtherCards() {
        for (int indexInHand = 0; indexInHand < cardsInHand.Length; indexInHand++) {
            if (cardsInHand[indexInHand] == null) {
                TryDrawCard(indexInHand);
            }
        }
    }

    private void ShuffleDeck() {
        cardsInDeck = cardsInDeck.OrderBy(card => UnityEngine.Random.value).ToList();
    }

    private void ShuffleDiscardToDeck() {
        cardsInDeck = cardsInDiscard.OrderBy(card => UnityEngine.Random.value).ToList();
        cardsInDiscard.Clear();
    }

    #endregion

    // debugging

    public void PrintAllCards(string startingText = "") {
        PrintCards(GetAllCards(), startingText);
    }

    private void PrintCards(List<ScriptableCardBase> cards, string startingText = "") {
        string whole = startingText;
        foreach (var card in cards) {
            if (card != null) {
                whole += card.GetName() + ", ";
            }
            else {
                whole += "Null";
            }
        }
        Debug.Log(whole);
    }

    private void PrintCards(ScriptableCardBase[] cards, string startingText = "") {
        string whole = startingText;
        foreach (var card in cards) {
            if (card != null) {
                whole += card.GetName() + ", ";
            }
            else {
                whole += "Null, ";
            }
        }
        Debug.Log(whole);
    }

    [Command]
    private void GainCard(string cardName) {
        ScriptableCardBase card = ResourceSystem.Instance.GetCardInstance(cardName);

        if (card == null) {
            Debug.LogWarning("Card Not Found!");
        }

        GainCard(card);
    }
}

public enum CardLocation {
    Deck,
    Discard,
    Hand,
    ModifierStack
}

[Serializable]
public struct CardAmount {
    public CardType CardType;
    public int Amount;
}
