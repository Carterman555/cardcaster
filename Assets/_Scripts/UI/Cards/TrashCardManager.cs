using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrashCardManager : StaticInstance<TrashCardManager> {

    public static event Action OnTrashCard;

    public void Activate() {
        PanelCardButton.OnClicked_PanelCard += ShowSelectButton;
        SelectButton.OnSelect_PanelCard += TrashCard;
    }
    public void Deactivate() {
        PanelCardButton.OnClicked_PanelCard -= ShowSelectButton;
        SelectButton.OnSelect_PanelCard -= TrashCard;
    }

    private void TrashCard(PanelCardButton panelCard) {
        panelCard.Trash();
        OnTrashCard?.Invoke();
    }

    private void ShowSelectButton(PanelCardButton panelCard) {
        Vector2 offset = new Vector2(0f, 205f);
        Vector2 position = (Vector2)panelCard.transform.position + offset;

        SelectButton.Instance.Show("Burn", position, panelCard);
    }
}
