using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllCardsPanel : StaticInstance<AllCardsPanel> {

    [SerializeField] private CardImage cardImagePrefab;

    [SerializeField] private Transform deckCardsContainer;
    [SerializeField] private Transform discardCardsContainer;
    [SerializeField] private Transform handCardsContainer;

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    [SerializeField] private MMF_Player popupPlayer;

    public void Open() {
        popupPlayer.SetDirectionBottomToTop();
        popupPlayer.PlayFeedbacks();
    }
    public void Close() {
        popupPlayer.SetDirectionTopToBottom();
        popupPlayer.PlayFeedbacks();
    }


    // played by popup feedback
    public void UpdateCards() {
        SetCardsInContainer(deckCardsContainer, DeckManager.Instance.GetCardsInDeck());
        SetCardsInContainer(discardCardsContainer, DeckManager.Instance.GetCardsInDiscard());
        SetCardsInContainer(handCardsContainer, DeckManager.Instance.GetCardsInHand());
    }

    private void SetCardsInContainer(Transform container, List<ScriptableCardBase> cards) {
        container.ReturnChildrenToPool();
        foreach (ScriptableCardBase card in cards) {
            CardImage newCardImage = cardImagePrefab.Spawn(container);
            newCardImage.Setup(card);
        }
    }
}
