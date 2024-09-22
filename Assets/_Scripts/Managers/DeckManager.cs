using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : Singleton<DeckManager> {

    public static event Action<float> OnEssenceChanged_Amount;

    [SerializeField] private int startDeckSize;
    [SerializeField] private int handSize;

    private List<ScriptableCardBaseOld> cardsInDeck = new();
    private List<ScriptableCardBaseOld> cardsInDiscard = new();
    private List<ScriptableCardBaseOld> cardsInHand = new();

    [SerializeField] private int maxEssence;
    private float essence;

    #region Get Methods

    public List<ScriptableCardBaseOld> GetCardsInDeck() {
        return cardsInDeck;
    }

    public List<ScriptableCardBaseOld> GetCardsInDiscard() {
        return cardsInDiscard;
    }

    public List<ScriptableCardBaseOld> GetCardsInHand() {
        return cardsInHand;
    }

    public List<ScriptableCardBaseOld> GetAllCards() {
        List<ScriptableCardBaseOld> allCards = new();
        allCards.AddRange(cardsInDeck);
        allCards.AddRange(cardsInDiscard);
        allCards.AddRange(cardsInHand);
        return allCards;
    }

    public float GetEssence() {
        return essence;
    }

    #endregion

    public void IncreaseEssence(float amount) {
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
            ScriptableCardBaseOld[] possibleStartingCards = ResourceSystem.Instance.GetAllCards().Where(card => card.IsPossibleStartingCard).ToArray();
            ScriptableCardBaseOld chosenCard = possibleStartingCards.RandomItem();
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

    public void UseCard(int indexInHand) {

        essence -= cardsInHand[indexInHand].GetCost();
        OnEssenceChanged_Amount?.Invoke(essence);

        cardsInDiscard.Add(cardsInHand[indexInHand]);

        DrawCard(indexInHand);
    }

    public void GainCard(ScriptableCardBaseOld card) {
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

    private void DrawCard(int indexInHand) {
        if (cardsInDeck.Count == 0) {
            ShuffleDiscardToDeck();
        }

        cardsInHand[indexInHand] = cardsInDeck[0];
        cardsInDeck.RemoveAt(0);
    }

    public void PrintAllCards(string startingText = "") {
        PrintCardsList(GetAllCards(), startingText);
    }

    private void PrintCardsList(List<ScriptableCardBaseOld> cards, string startingText = "") {
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
    Hand
}
