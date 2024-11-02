using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : StaticInstance<ShopUIManager>, IInitializable {

    public void Initialize() => Instance = this;

    public void Activate() {
        PanelCardButton.OnClicked_PanelCard += ShowSelectButton;
        SelectButton.OnSelect_PanelCard += ShowTradeUI;
    }
    public void Deactivate() {
        PanelCardButton.OnClicked_PanelCard -= ShowSelectButton;
        SelectButton.OnSelect_PanelCard -= ShowTradeUI;
    }

    private void ShowSelectButton(PanelCardButton panelCard) {
        Vector2 offset = new Vector2(0f, 205f);
        Vector2 position = (Vector2)panelCard.transform.position + offset;

        SelectButton.Instance.Show("Select", position, panelCard);
    }

    private void ShowTradeUI(PanelCardButton panelCard) {
        FeedbackPlayer.Play("OpenTradeUI");
    }
}
