using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllCardsPanel : MonoBehaviour {

    [SerializeField] private TrashCardButton trashCardPrefab;

    [SerializeField] private Transform deckCardsContainer;
    [SerializeField] private Transform discardCardsContainer;
    [SerializeField] private Transform handCardsContainer;

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    // played by popup feedback
    public void UpdateCards() {
        SetCardsInContainer(deckCardsContainer, DeckManager.Instance.GetCardsInDeck(), CardLocation.Deck);
        SetCardsInContainer(discardCardsContainer, DeckManager.Instance.GetCardsInDiscard(), CardLocation.Discard);
        SetCardsInContainer(handCardsContainer, DeckManager.Instance.GetCardsInHand(), CardLocation.Hand);
    }

    private void SetCardsInContainer(Transform container, List<ScriptableCardBase> cards, CardLocation cardLocation) {
        container.ReturnChildrenToPool();
        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++) {
            ScriptableCardBase card = cards[cardIndex];
            TrashCardButton newCard = trashCardPrefab.Spawn(container);
            newCard.Setup(card, cardLocation, cardIndex);
        }
    }
}
