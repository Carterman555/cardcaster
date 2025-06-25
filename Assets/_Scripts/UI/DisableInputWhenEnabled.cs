using UnityEngine;
using UnityEngine.InputSystem;

public class DisableInputWhenEnabled : MonoBehaviour {

    [SerializeField] private PlayerInput playerInput;

    private void OnEnable() {
        playerInput.enabled = false;
    }

    private void OnDisable() {
        playerInput.enabled = true;
    }

}
