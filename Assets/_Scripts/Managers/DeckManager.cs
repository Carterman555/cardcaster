using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckManager : Singleton<DeckManager> {

    // events for the cardUIManager to update the card UI
    public static event Action OnGainCardToHand;
    public static event Action<bool> OnTrashCardInHand;
    public static event Action OnReplaceCardInHand;
    public static event Action OnClearCards;

    public static event Action<int> OnMaxEssenceChanged_Amount;
    public static event Action<int> OnEssenceChanged_Amount;
    public static event Action<int> OnHandSizeChanged_Size;

    public static int MaxHandSize = 6;
    public int Essence { get; private set; }

    [SerializeField] private int startingCardAmount;

    private List<ScriptableCardBase> cardsInDeck = new();
    private List<ScriptableCardBase> cardsInDiscard = new();
    private ScriptableCardBase[] cardsInHand;
    private List<ScriptableCardBase> cardsInModifierStack = new();

    [SerializeField] private CardAmount[] startingCardPool;

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

    public List<ScriptableCardBase> GetAllCards() {
        List<ScriptableCardBase> allCards = new();
        allCards.AddRange(cardsInDeck);
        allCards.AddRange(cardsInDiscard);
        allCards.AddRange(cardsInHand);
        return allCards;
    }

    public int GetHandSize() {
        int handSize = cardsInHand.Where(card => card != null).Count();
        return handSize;
    }

    #endregion

    public void UpdateMaxEssence() {
        Essence = Mathf.Min(Essence, StatsManager.PlayerStats.MaxEssence);
        OnMaxEssenceChanged_Amount?.Invoke(StatsManager.PlayerStats.MaxEssence);
    }

    public void UpdateHandSize() {

        if (cardsInHand.Length == StatsManager.PlayerStats.HandSize) {
            return;
        }

        ScriptableCardBase[] oldHand = cardsInHand;
        cardsInHand = new ScriptableCardBase[StatsManager.PlayerStats.HandSize];

        int smallerLength = Mathf.Min(oldHand.Length, cardsInHand.Length);
        Array.Copy(oldHand, cardsInHand, smallerLength);

        bool handSizeIncreased = oldHand.Length < cardsInHand.Length;
        if (handSizeIncreased) {
            for (int handIndex = oldHand.Length; handIndex < cardsInHand.Length; handIndex++) {
                TryDrawCard(handIndex);
            }
        }
        else {
            for (int handIndex = cardsInHand.Length; handIndex < oldHand.Length; handIndex++) {
                cardsInDiscard.Add(oldHand[handIndex]);
            }
        }

        OnHandSizeChanged_Size?.Invoke(cardsInHand.Length);
    }

    public void ChangeEssenceAmount(int amount) {

        if (DebugManager.Instance.UnlimitedEssence) { // for debugging
            return;
        }

        Essence = Mathf.Clamp(Essence + amount, 0, StatsManager.PlayerStats.MaxEssence);
        OnEssenceChanged_Amount?.Invoke(Essence);
    }

    private void OnEnable() {
        GameSceneManager.OnStartGameLoadingStarted += OnStartGame;
        GameSceneManager.OnLevelComplete += OnLevelComplete;

        //debugging so can start game in 'game' scene
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game")) {
            ClearDeckAndEssence();
        }
    }

    private void OnDisable() {
        GameSceneManager.OnStartGameLoadingStarted -= OnStartGame;
        GameSceneManager.OnLevelComplete -= OnLevelComplete;
    }

    private void OnStartGame() {
        ClearDeckAndEssence();
        if (!GameSceneManager.Instance.InTutorial) {
            GiveStartingCards();
        }
    }

    private void OnLevelComplete(int obj) {
        // discard active modifier cards
        cardsInDiscard.AddRange(cardsInModifierStack);
        cardsInModifierStack.Clear();
    }

    public void ClearDeckAndEssence() {

        cardsInHand = new ScriptableCardBase[StatsManager.PlayerStats.HandSize];

        cardsInDeck.Clear();
        cardsInDiscard.Clear();
        cardsInModifierStack.Clear();

        OnClearCards?.Invoke(); // clears handcards

        Essence = StatsManager.PlayerStats.MaxEssence;

        OnEssenceChanged_Amount?.Invoke(Essence);
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

        // all the cards were gained to discard so put them in the deck
        ShuffleDiscardToDeck();
    }

    public void DiscardHandCard(int indexInHand) {
        ChangeEssenceAmount(-cardsInHand[indexInHand].Cost);

        cardsInDiscard.Add(cardsInHand[indexInHand]);
        cardsInHand[indexInHand] = null;

        TryDrawCard(indexInHand);
        TryDrawOtherCards();

        RemoveHandGaps();
    }

    public void StackHandCard(int indexInHand) {
        ChangeEssenceAmount(-cardsInHand[indexInHand].Cost);

        cardsInModifierStack.Add(cardsInHand[indexInHand]);
        cardsInHand[indexInHand] = null;

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

        if (GetHandSize() == StatsManager.PlayerStats.HandSize) {
            cardsInDiscard.Add(card);
        }
        else {
            cardsInHand[GetHandSize()] = card;
            OnGainCardToHand?.Invoke();
        }

        // if gains a locked card, unlock it
        bool cardLocked = !ResourceSystem.Instance.GetUnlockedCards().Any(c => c == card.CardType);
        if (cardLocked && !GameSceneManager.Instance.InTutorial) {
            ResourceSystem.Instance.UnlockCard(card.CardType);
            FeedbackPlayerOld.Play("NewCardUnlocked");
            NewCardUnlockedPanel.Instance.Setup(card);

            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.UnlockCard);
        }
    }

    public void TrashCard(CardLocation cardLocation, int cardIndex, bool usingCard) {

        if (cardLocation == CardLocation.Deck) {
            cardsInDeck[cardIndex].OnRemoved();
            cardsInDeck.RemoveAt(cardIndex);
        }
        else if (cardLocation == CardLocation.Discard) {
            cardsInDiscard[cardIndex].OnRemoved();
            cardsInDiscard.RemoveAt(cardIndex);
        }
        else if (cardLocation == CardLocation.Hand) {
            cardsInHand[cardIndex].OnRemoved();
            if (!TryDrawCard(cardIndex)) {
                cardsInHand[cardIndex] = null;
                RemoveHandGaps();
            }
            OnTrashCardInHand?.Invoke(usingCard);
        }
    }

    public void ReplaceCard(CardLocation cardLocation, int cardIndex, ScriptableCardBase newCard) {
        if (cardLocation == CardLocation.Deck) {
            cardsInDeck[cardIndex].OnRemoved();
            cardsInDeck[cardIndex] = newCard;
        }
        else if (cardLocation == CardLocation.Discard) {
            cardsInDiscard[cardIndex].OnRemoved();
            cardsInDiscard[cardIndex] = newCard;
        }
        else if (cardLocation == CardLocation.Hand) {
            cardsInHand[cardIndex].OnRemoved();
            cardsInHand[cardIndex] = newCard;
            OnReplaceCardInHand?.Invoke();
        }
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

    [Command]
    public void PrintAllCards(string startingText = "") {
        List<ScriptableCardBase> allCards = new();
        allCards.AddRange(cardsInDeck);
        allCards.AddRange(cardsInDiscard);
        allCards.AddRange(cardsInHand);
        allCards.AddRange(cardsInModifierStack);
        PrintCards(allCards, startingText);
    }

    private void PrintCards(List<ScriptableCardBase> cards, string startingText = "") {
        string whole = startingText;
        foreach (var card in cards) {
            if (card != null) {
                whole += card.Name + ", ";
            }
            else {
                whole += "Null, ";
            }
        }
        Debug.Log(whole);
    }

    private void PrintCards(ScriptableCardBase[] cards, string startingText = "") {
        string whole = startingText;
        foreach (var card in cards) {
            if (card != null) {
                whole += card.Name + ", ";
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
