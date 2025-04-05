using UnityEngine;

public class OpenURLButton : GameButton {

    [SerializeField] private string URL;

    protected override void OnClick() {
        base.OnClick();

        Application.OpenURL(URL);
    }

}
