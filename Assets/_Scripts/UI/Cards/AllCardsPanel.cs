using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class AllCardsPanel : StaticInstance<AllCardsPanel> {

    [SerializeField] private PanelCardButton panelCardPrefab;

    [SerializeField] private Transform deckCardsContainer;
    [SerializeField] private Transform discardCardsContainer;
    [SerializeField] private Transform handCardsContainer;

    public List<PanelCardButton> PanelCardButtons { get; private set; } = new();

    private void OnEnable() {
        UpdateCards();
    }

    // deactivate on the managers related to the AllCardsPanel that are possibly active. one is sometimes activated
    // when the AllCardsPanel is activated depending on why the AllCardsPanel is activated. (if to trash a card, the
    // trash manager is activated for example)
    private void OnDisable() {
        TrashCardManager.Instance.Deactivate();
        TradeUIManager.Instance.Deactivate();
    }

    private void UpdateCards() {
        PanelCardButtons.Clear();

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

            PanelCardButton newCard = panelCardPrefab.Spawn(container);
            newCard.Setup(card, cardLocation, cardIndex);

            PanelCardButtons.Add(newCard);
        }
    }

    #region Controller Input

    // played by trash and trade manager, so can select cards when they are active
    public IEnumerator MakeCardSelectable() {

        int framesToWait = 60;
        int frameCounter = 0;

        // wait until panel cards are spawned
        while (PanelCardButtons.Count == 0) {
            yield return null;

            frameCounter++;
            if (frameCounter > framesToWait) {
                Debug.LogWarning("Tried to make cards selectable, but cards didn't get spawned in time!");
                yield break;
            }
        }

        foreach (PanelCardButton panelCardButton in PanelCardButtons) {
            panelCardButton.GetComponent<Button>().interactable = true;
            print($"{panelCardButton.GetInstanceID()}: Enable button");
        }

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            PanelCardButtons[0].GetComponent<Button>().Select();
        }
    }

    public IEnumerator MakeCardSelectable(Rarity minRarity) {

        int framesToWait = 60;
        int frameCounter = 0;

        // wait until panel cards are spawned
        while (PanelCardButtons.Count == 0) {
            yield return null;

            frameCounter++;
            if (frameCounter > framesToWait) {
                Debug.LogWarning("Tried to make cards selectable, but cards didn't get spawned in time!");
                yield break;
            }
        }

        bool triedSelectCard = false;

        foreach (PanelCardButton panelCardButton in PanelCardButtons) {
            if (panelCardButton.GetCard().Rarity >= minRarity) {
                panelCardButton.GetComponent<Button>().interactable = true;

                // if using controller, select the first interable card
                if (!triedSelectCard) {
                    if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
                        panelCardButton.GetComponent<Button>().Select();
                    }
                    triedSelectCard = true;
                }
            }
            else {
                // fade out if can't trade card
                panelCardButton.GetComponentInChildren<CanvasGroup>().alpha = 0.65f;
            }
        }
    }

    #endregion
}
