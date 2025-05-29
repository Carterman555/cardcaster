using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : GameButton {

    protected override void OnClick() {
        base.OnClick();

        Time.timeScale = 1f;
        GameSceneManager.Instance.LoadMenu();
    }
}
