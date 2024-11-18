using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameButton : GameButton {

    protected override void OnClick() {
        base.OnClick();

        LevelManager.Instance.StartGame();
    }
}
