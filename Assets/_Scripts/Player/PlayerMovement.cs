using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : StaticInstance<PlayerMovement> {

    public static event Action<bool> OnChangedFacing; // bool: facing right

    [SerializeField] private ScriptablePlayer scriptablePlayer;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference dashAction;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;
    private bool isDashing;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();

        facingRight = true;
    }

    private void Update() {
        moveDirection = moveInput.action.ReadValue<Vector2>();

        if (moveDirection != Vector2.zero) {
            lastMoveDirection = moveDirection;
        }

        //if (dashAction.action.triggered && !isDashing) {
        if (Input.GetMouseButtonDown(1) && !isDashing) {
            StartCoroutine(Dash());
        }

        FaceTowardsMouse();
    }

    private void FixedUpdate() {
        if (!isDashing) {
            rb.velocity = moveDirection * scriptablePlayer.Stats.MoveSpeed;
        }
    }

    private IEnumerator Dash() {
        isDashing = true;
        rb.velocity = lastMoveDirection.normalized * scriptablePlayer.Stats.DashSpeed;
        yield return new WaitForSeconds(scriptablePlayer.Stats.DashTime);
        isDashing = false;
    }

    private bool facingRight;

    private void FaceTowardsMouse() {

        float mouseXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        bool mouseToRight = mouseXPos > transform.position.x;

        if (!facingRight && mouseToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            facingRight = true;
            OnChangedFacing?.Invoke(facingRight);
        }
        else if (facingRight && !mouseToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
            facingRight = false;
            OnChangedFacing?.Invoke(facingRight);
        }
    }
}
