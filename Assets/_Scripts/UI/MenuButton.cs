using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : GameButton {

    [SerializeField] private WarningPopup warningPopup;

    [SerializeField] private LocalizedString locWarningText;
    [SerializeField] private CanvasGroup canvasGroupToDisable;
    [SerializeField] private Button buttonToSelectOnClose;


    protected override void OnClick() {
        base.OnClick();

        warningPopup.Setup(locWarningText, canvasGroupToDisable, buttonToSelectOnClose);
        warningPopup.OnAccepted += SwitchToMenuScene;
    }

    private void SwitchToMenuScene() {
        warningPopup.OnAccepted -= SwitchToMenuScene;

        Time.timeScale = 1f;

        GameSceneManager.Instance.LoadMenu();
    }
}
