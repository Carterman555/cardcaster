using UnityEngine;

public class QuitButton : GameButton {

    protected override void OnClick() {
        base.OnClick();
        Quit();
    }

    private void Quit() {

        Application.Quit();

#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application
        Application.Quit();
#endif
    }
}
