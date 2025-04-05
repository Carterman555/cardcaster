using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Localization;

public class WarningPopup : MonoBehaviour {

    public event Action OnAccepted;

    [SerializeField] private TextMeshProUGUI warningText;

    private CanvasGroup canvasGroupToDisable;
    private Button buttonToSelectOnClose;

    public void Setup(LocalizedString warningTextStr, CanvasGroup canvasGroupToDisable, Button buttonToSelectOnClose = null) {
        warningText.text = warningTextStr.GetLocalizedString();
        this.canvasGroupToDisable = canvasGroupToDisable;
        this.buttonToSelectOnClose = buttonToSelectOnClose;

        OpenWarning();
    }

    // invoked by yes button
    public void OnAcceptButtonClicked() {
        OnAccepted?.Invoke();
        CloseWarning();
    }

    private void OpenWarning() {
        gameObject.SetActive(true);

        canvasGroupToDisable.interactable = false;
        canvasGroupToDisable.blocksRaycasts = false;

        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration: 0.3f).SetUpdate(true);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.OpenPanel);
    }

    public void CloseWarning() {

        transform.localScale = Vector3.one;
        transform.DOScale(0, duration: 0.3f).SetUpdate(true).OnComplete(() => {
            gameObject.SetActive(false);
            canvasGroupToDisable.interactable = true;
            canvasGroupToDisable.blocksRaycasts = true;

            if (buttonToSelectOnClose != null) {
                buttonToSelectOnClose.Select();
            }
        });

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ClosePanel);
    }
}
