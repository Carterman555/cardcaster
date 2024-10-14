using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AllCardsPanel : StaticInstance<AllCardsPanel> {

    [SerializeField] private PanelCardButton trashCardPrefab;

    [SerializeField] private Transform deckCardsContainer;
    [SerializeField] private Transform discardCardsContainer;
    [SerializeField] private Transform handCardsContainer;

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    private List<PanelCardButton> cardButtons = new();

    // played by popup feedback
    public void UpdateCards() {
        SetCardsInContainer(deckCardsContainer, DeckManager.Instance.GetCardsInDeck(), CardLocation.Deck);
        SetCardsInContainer(discardCardsContainer, DeckManager.Instance.GetCardsInDiscard(), CardLocation.Discard);
        SetCardsInContainer(handCardsContainer, DeckManager.Instance.GetCardsInHand().ToList(), CardLocation.Hand);
    }

    private void SetCardsInContainer(Transform container, List<ScriptableCardBase> cards, CardLocation cardLocation) {
        container.ReturnChildrenToPool();
        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++) {
            ScriptableCardBase card = cards[cardIndex];

            // could be null if deck ran out of cards to draw to hand
            if (card == null) {
                // TODO - if still want to add same spacing i could create spacing object and spawn it here
                continue;
            }

            PanelCardButton newCard = trashCardPrefab.Spawn(container);
            newCard.Setup(card, cardLocation, cardIndex);

            cardButtons.Add(newCard);
        }
    }

    public void SetCardToTrash(bool canTrash) {
        foreach (var card in cardButtons) {
            card.GetComponent<Button>().interactable = canTrash;
        }
    }
}
