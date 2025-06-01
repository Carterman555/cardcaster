using UnityEngine;

public class RebindPanel : MonoBehaviour {

    public static RebindPanel ActiveInstance { get; private set; }

    [SerializeField] private bool keyboard;
    public bool RebindingKeyboard => keyboard;

    private CanvasGroup canvasGroup;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable() {
        ActiveInstance = this;
    }

    private void OnDisable() {
        ActiveInstance = null;
    }

    public void EnableNavigation() {
        canvasGroup.interactable = true;
    }

    public void DisableNavigation() {
        canvasGroup.interactable = false;
    }

}
