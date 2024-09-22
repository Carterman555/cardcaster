using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsUIManager : StaticInstance<CardsUIManager> {

    [SerializeField] private CardButton cardButtonPrefab;
    private List<CardButton> cardButtons = new();

    [Header("Card Positioning")]
    [SerializeField] private RectTransform deckButtonTransform;
    [SerializeField] private float cardYPos;
    [SerializeField] private float cardSpacing;

    public void Setup() {

        List<ScriptableCardBaseOld> cardsInHand = DeckManager.Instance.GetCardsInHand();

        float cardXPos = -((cardsInHand.Count - 1) * cardSpacing * 0.5f);

        for (int i = 0; i < cardsInHand.Count; i++) {
            ScriptableCardBaseOld card = cardsInHand[i];
            CardButton cardButton = cardButtonPrefab.Spawn(transform);
            cardButton.Setup(i, deckButtonTransform, new Vector3(cardXPos, cardYPos));
            cardButton.DrawCard(card);

            cardXPos += cardSpacing;

            cardButtons.Add(cardButton);
        }
    }

    public void ReplaceCard(int index) {
        List<ScriptableCardBaseOld> cardsInHand = DeckManager.Instance.GetCardsInHand();

        cardButtons[index].DrawCard(cardsInHand[index]);
    }


}
