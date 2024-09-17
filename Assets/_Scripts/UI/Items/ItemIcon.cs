using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : GameButton {

    [SerializeField] private Image image;

    private ScriptableItemBase item;

    public void Setup(ScriptableItemBase item) {
        this.item = item;

        image.sprite = item.GetSprite();
    }

    protected override void OnClick() {
        base.OnClick();

        if (!ItemInfoPanel.Instance.gameObject.activeSelf) {
            FeedbackPlayer.Play("ShowItemInfo");
        }

        ItemInfoPanel.Instance.SetItem(item);
    }
}
