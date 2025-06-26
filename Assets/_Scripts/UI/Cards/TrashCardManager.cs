using System;
using UnityEngine;
using UnityEngine.Localization;

public class TrashCardManager : StaticInstance<TrashCardManager> {

    public static event Action OnTrashCard;
    public static event Action OnDeactivate;

    private bool active;

    private PanelCardButton panelCardToTrash;

    [SerializeField] private LocalizedString burnLocString;

    private bool trashedCard;

    public void Activate() {
        active = true;
        trashedCard = false;

        PanelCardButton.OnClicked_PanelCard += OnCardClicked;
        SelectButton.OnSelect_PanelCard += TrashCard;
        InputManager.OnControlSchemeChanged += TrySelectButton;

        //... can select cards if trashing card
        StartCoroutine(AllCardsPanel.Instance.MakeCardSelectable());
    }

    public void Deactivate() {
        active = false;

        PanelCardButton.OnClicked_PanelCard -= OnCardClicked;
        SelectButton.OnSelect_PanelCard -= TrashCard;
        InputManager.OnControlSchemeChanged -= TrySelectButton;

        panelCardToTrash = null;

        OnDeactivate?.Invoke();
    }

    public bool IsActive() {
        return active;
    }

    private void OnCardClicked(PanelCardButton panelCard) {

        if (trashedCard) {
            return;
        }

        if (panelCardToTrash != panelCard) {
            panelCardToTrash = panelCard;
            SelectButton.Instance.Show(burnLocString, panelCard);
        }
    }

    private void TrashCard(PanelCardButton panelCard) {

        if (trashedCard) {
            return;
        }

        panelCard.Trash();

        panelCardToTrash = null;
        trashedCard = true;

        SelectButton.Instance.Hide();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BurnCard);

        OnTrashCard?.Invoke();
    }

    private void TrySelectButton() {
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            StartCoroutine(AllCardsPanel.Instance.MakeCardSelectable());
        }
    }
}
