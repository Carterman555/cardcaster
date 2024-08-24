using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : StaticInstance<PlayerMovement>, IHasStats, IChangesFacing {

    public event Action<bool> OnChangedFacing; // bool: facing right
    
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference dashAction;

    private Rigidbody2D rb;
    private Knockback knockback;

    private Vector2 moveDirection;
    private bool isDashing;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();

        facingRight = true;
    }

    private void Update() {
        moveDirection = moveInput.action.ReadValue<Vector2>();

        //if (dashAction.action.triggered && !isDashing) {
        if (Input.GetMouseButtonDown(1) && !isDashing && !knockback.IsApplyingKnockback()) {
            StartCoroutine(Dash());
        }

        FaceTowardsMouse();
    }

    private void FixedUpdate() {
        if (!isDashing && !knockback.IsApplyingKnockback()) {
            rb.velocity = moveDirection * stats.MoveSpeed;
        }
    }

    private IEnumerator Dash() {
        isDashing = true;
        rb.velocity = moveDirection.normalized * stats.DashSpeed;
        yield return new WaitForSeconds(stats.DashTime);
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
