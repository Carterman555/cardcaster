using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : Singleton<DeckManager> {

    public static event Action<float> OnEssenceChanged_Amount;

    [SerializeField] private int startDeckSize;
    [SerializeField] private int handSize;

    private List<ScriptableCardBase> cardsInDeck = new();
    private List<ScriptableCardBase> cardsInDiscard = new();
    private List<ScriptableCardBase> cardsInHand = new();
    private List<ScriptableCardBase> cardsInModifierStack = new();

    [SerializeField] private int maxEssence;
    private float essence;

    #region Get Methods

    public List<ScriptableCardBase> GetCardsInDeck() {
        return cardsInDeck;
    }

    public List<ScriptableCardBase> GetCardsInDiscard() {
        return cardsInDiscard;
    }

    public List<ScriptableCardBase> GetCardsInHand() {
        return cardsInHand;
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

    public void ChangeEssenceAmount(float amount) {
        essence = Mathf.MoveTowards(essence, maxEssence, amount);

        OnEssenceChanged_Amount?.Invoke(essence);
    }

    private void Start() {
        ChooseStartingDeck();

        ShuffleDeck();
        DrawStartingHand();

        essence = maxEssence;

        OnEssenceChanged_Amount?.Invoke(essence);
    }

    private void ChooseStartingDeck() {
        for (int i = 0; i < startDeckSize; i++) {
            ScriptableCardBase[] possibleStartingCards = ResourceSystem.Instance.GetAllCards().Where(card => card.IsPossibleStartingCard).ToArray();
            ScriptableCardBase chosenCard = possibleStartingCards.RandomItem();
            cardsInDeck.Add(Instantiate(chosenCard));
        }
    }

    private void ShuffleDeck() {
        cardsInDeck = cardsInDeck.OrderBy(card => UnityEngine.Random.value).ToList();
    }

    private void DrawStartingHand() {
        cardsInHand = cardsInDeck.Take(handSize).ToList();
        cardsInDeck = cardsInDeck.Skip(handSize).ToList();

        CardsUIManager.Instance.Setup();
    }

    public void UseAbilityCard(int indexInHand) {

        ChangeEssenceAmount(-cardsInHand[indexInHand].GetCost());

        AddCardToDiscard(indexInHand);

        DrawCard(indexInHand);
    }

    public void AddCardToDiscard(int indexInHand) {
        cardsInDiscard.Add(cardsInHand[indexInHand]);
    }

    public void AddCardToStack(int indexInHand) {
        cardsInModifierStack.Add(cardsInHand[indexInHand]);
    }

    public void GainCard(ScriptableCardBase card) {
        cardsInDiscard.Add(card);
    }

    public void TrashCard(CardLocation cardLocation, int cardIndex) {
        if (cardLocation == CardLocation.Deck) {
            cardsInDeck.RemoveAt(cardIndex);
        }
        else if (cardLocation == CardLocation.Discard) {
            cardsInDiscard.RemoveAt(cardIndex);
        }
        else if (cardLocation == CardLocation.Hand) {
            DrawCard(cardIndex);
        }
    }

    public void DrawCard(int indexInHand) {
        if (cardsInDeck.Count == 0) {
            ShuffleDiscardToDeck();
        }

        cardsInHand[indexInHand] = cardsInDeck[0];
        cardsInDeck.RemoveAt(0);
    }

    public void PrintAllCards(string startingText = "") {
        PrintCardsList(GetAllCards(), startingText);
    }

    private void PrintCardsList(List<ScriptableCardBase> cards, string startingText = "") {
        string whole = startingText;
        foreach (var card in cards) {
            whole += card.GetName() + ", ";
        }
        Debug.Log(whole);
    }

    private void ShuffleDiscardToDeck() {
        cardsInDeck = cardsInDiscard.OrderBy(card => UnityEngine.Random.value).ToList();
        cardsInDiscard.Clear();
    }
}

public enum CardLocation {
    Deck,
    Discard,
    Hand,
    ModifierStack
}
