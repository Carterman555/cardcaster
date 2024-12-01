using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : GameButton {

    [SerializeField] private CanvasGroup canvasGroupToDisable;

    protected override void OnClick() {
        base.OnClick();

        string warningText = "This will reset your progress. Are you sure you want to exit?";
        WarningPopup.Instance.Setup(warningText, canvasGroupToDisable);

        WarningPopup.Instance.OnAccepted += SwitchToMenuScene;
    }

    private void SwitchToMenuScene() {
        WarningPopup.Instance.OnAccepted -= SwitchToMenuScene;

        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }
}
