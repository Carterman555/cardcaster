using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour {

    [SerializeField] private InputActionReference attackInput;

    private void Update() {
        //if (attackInput.action.triggered) {
        if (Input.GetMouseButtonDown(0)) {
        }
    }
}
