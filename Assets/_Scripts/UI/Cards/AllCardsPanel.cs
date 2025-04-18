using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AllCardsPanel : StaticInstance<AllCardsPanel> {

    [SerializeField] private PanelCardButton panelCardPrefab;

    [SerializeField] private Transform deckCardsContainer;
    [SerializeField] private Transform discardCardsContainer;
    [SerializeField] private Transform handCardsContainer;

    public List<PanelCardButton> PanelCardButtons { get; private set; } = new();

    private void OnEnable() {
        UpdateCards();

        if (toSetupControllerInput) {
            TrySetupControllerCardSelection();
        }
    }

    // deactivate on the managers related to the AllCardsPanel that are possibly active. one is sometimes activated
    // when the AllCardsPanel is activated depending on why the AllCardsPanel is activated. (if to trash a card, the
    // trash manager is activated for example)
    private void OnDisable() {
        TrashCardManager.Instance.Deactivate();
        TradeUIManager.Instance.Deactivate();

        toSetupControllerInput = false;
    }

    private void UpdateCards() {
        SetCardsInContainer(deckCardsContainer, DeckManager.Instance.GetCardsInDeck(), CardLocation.Deck);
        SetCardsInContainer(discardCardsContainer, DeckManager.Instance.GetCardsInDiscard(), CardLocation.Discard);
        SetCardsInContainer(handCardsContainer, DeckManager.Instance.GetCardsInHand().ToList(), CardLocation.Hand);
    }

    private void SetCardsInContainer(Transform container, List<ScriptableCardBase> cards, CardLocation cardLocation) {
        container.ReturnChildrenToPool();

        PanelCardButtons.Clear();

        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++) {
            ScriptableCardBase card = cards[cardIndex];

            // could be null if deck ran out of cards to draw to hand
            if (card == null) {
                // TODO - if still want to add same spacing i could create spacing object and spawn it here
                continue;
            }

            PanelCardButton newCard = panelCardPrefab.Spawn(container);
            newCard.Setup(card, cardLocation, cardIndex);

            PanelCardButtons.Add(newCard);
        }
    }

    #region Controller Input

    private bool toSetupControllerInput;

    // played by outside script and in setup, so can be play after or before the cards are setup and it'll still work
    public void TrySetupControllerCardSelection() {

        // only setup card if using controller
        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Controller) {
            return;
        }

        if (PanelCardButtons.Count > 0) {
            SetupControllerCardSelection();
        }
        else {
            toSetupControllerInput = true;
        }
    }

    private void SetupControllerCardSelection() {

        //... select the first card
        PanelCardButtons[0].GetComponent<Button>().Select();
    }

    #endregion
}
