using UnityEngine;
using UnityEngine.UI;

public class DisabledButtonFade : MonoBehaviour {

    [SerializeField] private CanvasGroup cardImageCanvasGroup;
    [SerializeField] private float deactiveFade;

    private bool faded;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void Update() {
        if (!faded && !button.interactable) {
            cardImageCanvasGroup.alpha = deactiveFade;
            faded = true;
        }
        else if (faded && button.interactable) {
            cardImageCanvasGroup.alpha = 1f;
            faded = false;
        }
    }
}
