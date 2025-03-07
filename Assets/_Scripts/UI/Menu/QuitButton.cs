using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : GameButton {

    [SerializeField] private CanvasGroup canvasGroupToDisable;

    protected override void OnClick() {
        base.OnClick();

        WarningPopup.Instance.Setup("Are you sure you want to quit?", canvasGroupToDisable);

        WarningPopup.Instance.OnAccepted += Quit;
    }

    private void Quit() {
        WarningPopup.Instance.OnAccepted -= Quit;

#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application
        Application.Quit();
#endif
    }
}
