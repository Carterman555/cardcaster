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
        //ChooseStartingDeck();
        //ShuffleDeck();
        //DrawStartingHand();
        
        SetupEmptyHand();

        essence = maxEssence;

        OnEssenceChanged_Amount?.Invoke(essence);
    }

    #region To delete (if starting with empty hand)
    private void ChooseStartingDeck() {
        for (int i = 0; i < startDeckSize; i++) {
            ScriptableCardBase[] possibleStartingCards = ResourceSystem.Instance.GetAllCards().Where(card => card.IsPossibleStartingCard).ToArray();
            ScriptableCardBase chosenCard = possibleStartingCards.RandomItem();
            cardsInDeck.Add(Instantiate(chosenCard));
        }
    }

    private void DrawStartingHand() {
        cardsInHand = cardsInDeck.Take(handSize).ToList();
        cardsInDeck = cardsInDeck.Skip(handSize).ToList();
    }

    #endregion

    private void SetupEmptyHand() {
        cardsInHand = new();
        for (int i = 0; i < handSize; i++) {
            cardsInHand.Add(null);
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
        TryDrawCard(indexInHand);
        TryDrawOtherCards();
    }

    #region Basic Deck Methods

    public void DiscardStackedCards() {
        cardsInDiscard.AddRange(cardsInModifierStack);
        cardsInModifierStack.Clear();
    }

    public void GainCard(ScriptableCardBase card) {

        // gain a card to the discard
        cardsInDiscard.Add(card);

        // if the hand doesn't have the amount of cards it can, draw more
        TryDrawOtherCards();
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
        for (int indexInHand = 0; indexInHand < cardsInHand.Count; indexInHand++) {
            if (cardsInHand[indexInHand] == null) {
                bool drewCard = TryDrawCard(indexInHand);
                if (drewCard) {
                    CardsUIManager.Instance.DrawCard(indexInHand);
                }
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


}

public enum CardLocation {
    Deck,
    Discard,
    Hand,
    ModifierStack
}
