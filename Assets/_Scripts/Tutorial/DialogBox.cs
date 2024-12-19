using Febucci.UI;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] private TextMeshProUGUI enterText;

    public void ShowText(string text, bool showEnterText = true) {
        if (!gameObject.activeSelf) {
            FeedbackPlayer.Play("DialogBox");
        }

        dialogText.text = text;
        dialogText.GetComponent<TypewriterByCharacter>().StartShowingText();

        enterText.enabled = showEnterText;
    }

    public void HideBox() {
        FeedbackPlayer.PlayInReverse("DialogBox");
    }
}
