using Febucci.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class DialogBox : MonoBehaviour, IInitializable {

    #region Static Instance

    public static DialogBox Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI nextDialogText;

    [SerializeField] private InputActionReference nextDialogAction;

    private LocalizedString locText;
    private bool showNextDialogText;

    public void ShowText(LocalizedString locText, bool showNextDialogText = true, InputActionReference dialogAction = null) {
        this.locText = locText;
        this.showNextDialogText = showNextDialogText;

        if (!gameObject.activeSelf) {
            FeedbackPlayerReference.Play("DialogBox");
        }

        locText.StringChanged += UpdateText;

        string text = locText.GetLocalizedString();
        if (dialogAction != null) {
            string actionText = InputManager.Instance.GetBindingText(dialogAction, shortDisplayName: false);
            text = text.Replace("{ACTION}", actionText);
        }

        dialogText.text = text;
        dialogText.GetComponent<TypewriterByCharacter>().StartShowingText();

        string inputStr = InputManager.Instance.GetBindingText(nextDialogAction, shortDisplayName: false);
        nextDialogText.text = "[" + inputStr + "]";
        if (!showNextDialogText) {
            nextDialogText.text = "";
        }
    }

    public void Hide() {
        locText.StringChanged += UpdateText;

        if (gameObject.activeSelf) {
            FeedbackPlayerReference.Play("DialogBox");
        }
    }

    private void UpdateText(string value) {
        dialogText.text = value;

        string inputStr = InputManager.Instance.GetBindingText(nextDialogAction, shortDisplayName: false);
        nextDialogText.text = "[" + inputStr + "]";
        if (!showNextDialogText) {
            nextDialogText.text = "";
        }
    }
}
