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

    // played by popup feedback
    public void SetupCardPanel() {
        UpdateCards();
        TrySelectFirstCard();
    }

    private void UpdateCards() {
        SetCardsInContainer(deckCardsContainer, DeckManager.Instance.GetCardsInDeck(), CardLocation.Deck);
        SetCardsInContainer(discardCardsContainer, DeckManager.Instance.GetCardsInDiscard(), CardLocation.Discard);
        SetCardsInContainer(handCardsContainer, DeckManager.Instance.GetCardsInHand().ToList(), CardLocation.Hand);
    }

    private void TrySelectFirstCard() {
        // TODO - finish
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
        }
    }

    // deactivate on the managers related to the AllCardsPanel that are possibly active. one is sometimes activated
    // when the AllCardsPanel is activated depending on why the AllCardsPanel is activated. (if to trash a card, the
    // trash manager is activated for example)
    private void OnDisable() {
        TrashCardManager.Instance.Deactivate();
        ShopUIManager.Instance.Deactivate();
    }
}
