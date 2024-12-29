using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ChestItemInfoUI : MonoBehaviour, IInitializable {

    #region Static Instance

    public static ChestItemInfoUI Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    [SerializeField] private CardImage cardImage;
    [SerializeField] private GameObject heal;

    private ItemInfo itemInfoShowing;
    private ItemInfo itemInfoToShow;

    public void SetCardInfo(ScriptableCardBase card) {
        itemInfoToShow = new() { Heal = false, Card = card };
        gameObject.SetActive(true);
    }

    public void SetHealInfo() {
        itemInfoToShow = new() { Heal = true, Card = null };
        gameObject.SetActive(true);
    }

    public void RemoveInfo() {
        itemInfoToShow = null;
    }

    private void Update() {

        MMF_Player itemInfoPlayer = FeedbackPlayer.GetPlayer("ChestItemInfoPopup");

        bool panelMoving = itemInfoPlayer.IsPlaying;

        // everytime the card switches info, it has to be in the bottom pos
        bool hoveringItem = itemInfoToShow != null;
        if (hoveringItem && PanelAtBottomPos()) {
            itemInfoPlayer.PlayFeedbacks();
            SetInfo(itemInfoToShow);
        }

        //... if not showing the info of the item the player is on
        if (!IsPanelShowingInfo(itemInfoToShow)) {

            // if panel is moving up, move the panel down by reverting feedback (but don't set info yet, it will set at bottom pos)
            bool panelMovingUp = panelMoving && itemInfoPlayer.Direction == MMFeedbacks.Directions.TopToBottom;
            if (panelMovingUp) {
                itemInfoPlayer.Revert();
            }

            // if panel is at top, move the panel down by playing feedback (but don't set info yet, it will set at bottom pos)
            if (PanelAtTopPos()) {
                itemInfoPlayer.PlayFeedbacks();
            }
        }
    }

    private bool IsPanelShowingInfo(ItemInfo itemInfo) {

        if (itemInfo == null || itemInfoShowing == null) {
            return false;
        }

        bool sameHeal = itemInfo.Heal == itemInfoShowing.Heal;

        // sameCard is true if the card types are the same or they're both null
        bool sameCard = false;
        if (itemInfo.Card != null && itemInfoShowing.Card != null) {
            sameCard = itemInfo.Card.CardType == itemInfoShowing.Card.CardType;
        }
        else if (itemInfo.Card == null && itemInfo.Card == null) {
            sameCard = true;
        }

        return sameHeal && sameCard;
    }

    private void SetInfo(ItemInfo itemInfo) {

        itemInfoShowing = itemInfo;

        cardImage.gameObject.SetActive(!itemInfo.Heal);
        heal.SetActive(itemInfo.Heal);

        if (!itemInfo.Heal) {
            cardImage.Setup(itemInfo.Card);
        }
    }


    private bool PanelAtTopPos() {
        MMF_Player itemInfoPlayer = FeedbackPlayer.GetPlayer("ChestItemInfoPopup");

        if (itemInfoPlayer.IsPlaying) {
            return false;
        }

        Vector2 topPos = itemInfoPlayer.GetFeedbackOfType<MMF_Position>().DestinationPosition;
        Vector2 currentPos = GetComponent<RectTransform>().anchoredPosition;

        bool inTopPos = Vector2.Distance(topPos, currentPos) < 0.05f;
        return inTopPos;
    }

    private bool PanelAtBottomPos() {
        MMF_Player itemInfoPlayer = FeedbackPlayer.GetPlayer("ChestItemInfoPopup");

        if (itemInfoPlayer.IsPlaying) {
            return false;
        }

        Vector2 botPos = itemInfoPlayer.GetFeedbackOfType<MMF_Position>().InitialPosition;
        Vector2 currentPos = GetComponent<RectTransform>().anchoredPosition;

        bool inBottomPos = Vector2.Distance(botPos, currentPos) < 0.05f;
        return inBottomPos;
    }
}

public class ItemInfo {
    public bool Heal;
    public ScriptableCardBase Card;
}
