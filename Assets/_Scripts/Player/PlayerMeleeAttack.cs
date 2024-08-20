using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour {

    public static event Action OnAttack;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform slashPrefab;

    private void Update() {
        //if (attackInput.action.triggered) {
        if (Input.GetMouseButtonDown(0)) {
            Attack();
        }
    }

    private void Attack() {
        Vector2 toMouseDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        slashPrefab.Spawn(transform.position, toMouseDirection.DirectionToRotation(), Containers.Instance.Effects);

        OnAttack?.Invoke();
    }
}
