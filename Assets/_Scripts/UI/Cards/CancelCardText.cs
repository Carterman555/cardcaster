using UnityEngine;
using UnityEngine.InputSystem;

public class CancelCardText : MonoBehaviour, IInitializable {

    [SerializeField] private InputActionReference cancelCardAction;

    public string Input; // for localize string event (it sets the text)

    // to make sure Input gets set before the localize string event sets the string
    public void Initialize() {
        Input = InputManager.Instance.GetBindingText(cancelCardAction);
    }

    private void OnEnable() {
        Input = InputManager.Instance.GetBindingText(cancelCardAction);
    }
}
