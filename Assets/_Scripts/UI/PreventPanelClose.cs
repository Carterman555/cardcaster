using System.Collections;
using UnityEngine;

public class PreventPanelClose : MonoBehaviour {

    [SerializeField] private bool withDelay;

    private void OnEnable() {
        ClosablePanel.DisallowClosing();
    }

    private void OnDisable() {

        if (Helpers.GameStopping()) {
            return;
        }

        if (withDelay) {
            InputManager.Instance.StartCoroutine(DelayedAllowClosing());
        }
        else {
            ClosablePanel.AllowClosing();
        }
    }

    private IEnumerator DelayedAllowClosing() {
        yield return new WaitForSeconds(0.3f);

        ClosablePanel.AllowClosing();
    }
}
