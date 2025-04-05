using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool showing;

    public void ShowText(string text, bool showNextDialogText = true) {
        if (!gameObject.activeSelf) {
            FeedbackPlayerReference.Play("DialogBox");
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
        if (gameObject.activeSelf) {
            FeedbackPlayerReference.Play("DialogBox");
        }
    }
}
