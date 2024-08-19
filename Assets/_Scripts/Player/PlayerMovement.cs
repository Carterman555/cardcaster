using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private ScriptablePlayerMovement moveSettings;
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference dashAction;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;
    private bool isDashing;

    private void Awake() {
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
            rb.velocity = moveDirection * moveSettings.Speed;
        }
    }

    private IEnumerator Dash() {
        isDashing = true;
        rb.velocity = lastMoveDirection.normalized * moveSettings.DashSpeed;
        yield return new WaitForSeconds(moveSettings.DashTime);
        isDashing = false;
    }

    private bool facingRight;

    private void FaceTowardsMouse() {

        float mouseXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        bool mouseToRight = mouseXPos > transform.position.x;

        if (!facingRight && mouseToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            facingRight = true;
        }
        else if (facingRight && !mouseToRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
            facingRight = false;
        }
    }
}
