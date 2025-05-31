using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : GameButton {

    protected override void OnClick() {
        base.OnClick();

        GameSceneManager.Instance.StartGame(tutorial: true);
        Tutorial.ResetPlayerDied();
    }
}
