using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour {

    [SerializeField] private InputActionReference attackInput;

    private void OnEnable() {
        attackInput.action.started += TryAttack;
    }
    private void OnDisable() {
        attackInput.action.started -= TryAttack;
    }

    private void TryAttack(InputAction.CallbackContext context) {
        print("attack");
    }
}
