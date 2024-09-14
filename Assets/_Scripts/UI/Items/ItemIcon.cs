using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : GameButton {

    private static bool panelOpen;

    [SerializeField] private Image image;

    private ScriptableItemBase item;

    public void Setup(ScriptableItemBase item) {
        this.item = item;

        image.sprite = item.GetSprite();
    }

    protected override void OnClick() {
        base.OnClick();

        //panelOpen = true;

        if (!ItemInfoPanel.Instance.gameObject.activeSelf) {
            FeedbackPlayer.Play("ShowItemInfo");
        }

        ItemInfoPanel.Instance.SetItem(item);
    }

    public void SetPanelClosed() {
        panelOpen = false;
    }
}
