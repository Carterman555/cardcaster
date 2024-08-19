using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private ScriptablePlayerMovement playerMoveSettings;
    [SerializeField] private InputActionReference move;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        moveDirection = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        rb.velocity = moveDirection * playerMoveSettings.Speed;
    }
}
