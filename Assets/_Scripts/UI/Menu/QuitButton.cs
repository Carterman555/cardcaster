using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class QuitButton : GameButton {

    [SerializeField] private CanvasGroup canvasGroupToDisable;
    [SerializeField] private WarningPopup warningPopup;

    [SerializeField] private LocalizedString locWarning;

    protected override void OnClick() {
        base.OnClick();

        warningPopup.Setup(locWarning, canvasGroupToDisable);
        warningPopup.OnAccepted += Quit;
    }

    private void Quit() {
        warningPopup.OnAccepted -= Quit;

#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application
        Application.Quit();
#endif
    }
}
