using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : StaticInstance<PlayerMovement>, IHasStats, IChangesFacing {

    [SerializeField] private InputActionReference moveInput;

    private Rigidbody2D rb;
    private Knockback knockback;

    private Vector2 moveDirection;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        attackMeleeAttack = GetComponent<PlayerMeleeAttack>();

        facingRight = true;
        stoppedFromAttack = false;
    }

    private void Update() {
        HandleStoppedFromAttack();

        if (stoppedFromAttack) {
            return;
        }

        moveDirection = moveInput.action.ReadValue<Vector2>();

        //if (dashAction.action.triggered && !isDashing) {
        if (Input.GetMouseButtonDown(1) && !isDashing && !knockback.IsApplyingKnockback()) {
            StartCoroutine(Dash());
        }

        FaceTowardsMouse();
    }

    private void FixedUpdate() {

        if (stoppedFromAttack) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!isDashing && !knockback.IsApplyingKnockback()) {
            rb.velocity = moveDirection * stats.MoveSpeed;
        }
    }

    #region Dash

    [SerializeField] private InputActionReference dashAction;

    private bool isDashing;

    private IEnumerator Dash() {
        isDashing = true;
        rb.velocity = moveDirection.normalized * stats.DashSpeed;
        yield return new WaitForSeconds(stats.DashTime);
        isDashing = false;
    }

    #endregion

    #region Face Towards Mouse

    public event Action<bool> OnChangedFacing; // bool: facing right

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

    #endregion

    #region Stop When Attacking

    [SerializeField] private float attackStopDuration = 0.3f;
    private float attackStopTimer;
    private bool stoppedFromAttack;

    private PlayerMeleeAttack attackMeleeAttack;

    private void OnEnable() {
        attackMeleeAttack.OnAttack += StopFromAttack;
    }
    private void OnDisable() {
        attackMeleeAttack.OnAttack -= StopFromAttack;
    }

    private void StopFromAttack() {
        stoppedFromAttack = true;
        attackStopTimer = 0;
    }

    private void HandleStoppedFromAttack() {
        attackStopTimer += Time.deltaTime;
        if (attackStopTimer > attackStopDuration) {
            stoppedFromAttack = false;
        }
    }

    #endregion
}
