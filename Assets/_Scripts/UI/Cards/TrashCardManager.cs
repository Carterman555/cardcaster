using System;
using UnityEngine;
using UnityEngine.Localization;

public class TrashCardManager : StaticInstance<TrashCardManager> {

    public static event Action OnTrashCard;

    private bool active;

    private PanelCardButton panelCardToTrash;

    [SerializeField] private LocalizedString burnLocString;

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
    }

    private void ShowSelectButton(PanelCardButton panelCard) {
        SelectButton.Instance.Show(burnLocString, panelCard);
    }

    private void TrashCard(PanelCardButton panelCard) {
        panelCard.Trash();

        panelCardToTrash = null;

        SelectButton.Instance.Hide();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BurnCard);

        OnTrashCard?.Invoke();
    }
}
