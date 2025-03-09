using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrashCardManager : StaticInstance<TrashCardManager> {

    public static event Action OnTrashCard;

    private bool active;

    private PanelCardButton panelCardToTrash;

    public void Activate() {
        active = true;

        PanelCardButton.OnClicked_PanelCard += OnCardClicked;
        SelectButton.OnSelect_PanelCard += TrashCard;

        //... can select cards if trashing card
        AllCardsPanel.Instance.TrySetupControllerCardSelection();
    }
    public void Deactivate() {
        active = false;

        PanelCardButton.OnClicked_PanelCard -= OnCardClicked;
        SelectButton.OnSelect_PanelCard -= TrashCard;

        panelCardToTrash = null;
    }

    public bool IsActive() {
        return active;
    }

    private void OnCardClicked(PanelCardButton panelCard) {

        // if clicked the card for the first time
        if (panelCardToTrash != panelCard) {
            panelCardToTrash = panelCard;
            ShowSelectButton(panelCard);
        }
        // if clicked the card a second time, trash it
        else {
            TrashCard(panelCard);
        }
    }

    private void ShowSelectButton(PanelCardButton panelCard) {
        SelectButton.Instance.Show("Burn", panelCard);
    }

    private void TrashCard(PanelCardButton panelCard) {
        panelCard.Trash();

        panelCardToTrash = null;

        SelectButton.Instance.Hide();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BurnCard);

        OnTrashCard?.Invoke();
    }
}
