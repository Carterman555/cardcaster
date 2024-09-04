using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : Singleton<DeckManager> {

    public static event Action<float> OnEssenceChanged_Fraction;

    [SerializeField] private int startDeckSize;
    [SerializeField] private int handSize;

    private List<ScriptableCardBase> cardsInDeck = new();
    private List<ScriptableCardBase> cardsInDiscard = new();
    private List<ScriptableCardBase> cardsInHand = new();

    [SerializeField] private float maxEssence;
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

    public float GetEssenceFraction() {
        return essence / maxEssence;
    }

    #endregion

    public void IncreaseEssence() {
        float essenceIncrease = 1f;
        essence = Mathf.MoveTowards(essence, maxEssence, essenceIncrease);

        OnEssenceChanged_Fraction?.Invoke(GetEssenceFraction());
    }

    private void Start() {
        ChooseStartingDeck();

        ShuffleDeck();
        DrawStartingHand();

        essence = maxEssence;

        OnEssenceChanged_Fraction?.Invoke(GetEssenceFraction());
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

        //CardsUIManager.Instance.Setup();
    }

    public void UseCard(int indexInHand) {

        essence -= cardsInHand[indexInHand].GetCost();
        OnEssenceChanged_Fraction?.Invoke(GetEssenceFraction());

        cardsInDiscard.Add(cardsInHand[indexInHand]);

        DrawCard(indexInHand);
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

    private void DrawCard(int indexInHand) {
        if (cardsInDeck.Count == 0) {
            ShuffleDiscardToDeck();
        }

        cardsInHand[indexInHand] = cardsInDeck[0];
        cardsInDeck.RemoveAt(0);

        //CardsUIManager.Instance.ReplaceCard(indexInHand);
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
    Hand
}
