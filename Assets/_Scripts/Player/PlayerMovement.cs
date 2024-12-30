using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : StaticInstance<PlayerMovement>, IHasStats, IChangesFacing {


    [SerializeField] private InputActionReference moveInput;

    private Rigidbody2D rb;
    private Knockback knockback;

    private Vector2 moveDirection;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    [SerializeField] private Animator anim;

    private float stepTimer;

    protected override void Awake() {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        attackMeleeAttack = GetComponent<PlayerMeleeAttack>();

        facingRight = true;
        stoppedFromAttack = false;
    }

    private void OnEnable() {
        attackMeleeAttack.OnAttack += StopFromAttack;
    }
    private void OnDisable() {
        attackMeleeAttack.OnAttack -= StopFromAttack;
        rb.velocity = Vector2.zero;
    }

    private void Update() {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        HandleStoppedFromAttack();

        bool moving = rb.velocity.magnitude > 0;

        anim.SetBool("move", moving);

        if (stoppedFromAttack) {
            return;
        }

        moveDirection = moveInput.action.ReadValue<Vector2>();
        
        if (dashAction.action.triggered && !isDashing && !knockback.IsApplyingKnockback() && moveDirection.magnitude > 0f) {
            StartCoroutine(Dash());
        }

        FaceTowardsMouse();

        HandleStepSounds(moving);
    }

    private bool wasMoving;

    private void HandleStepSounds(bool moving) {

        bool startedMoving = !wasMoving && moving;
        wasMoving = moving;

        if (startedMoving) {
            stepTimer = 1f;
        }

        if (moving) {
            float stepCooldown = 0.2f;
            stepTimer += Time.deltaTime;
            if (stepTimer > stepCooldown) {
                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.PlayerStep);
                stepTimer = 0;
            }
        }
    }

    private void FixedUpdate() {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (stoppedFromAttack) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!isDashing && !knockback.IsApplyingKnockback()) {
            rb.velocity = moveDirection * stats.MoveSpeed;
        }
    }

    #region Dash

    public UnityEvent OnDash;

    [SerializeField] private InputActionReference dashAction;

    private bool isDashing;

    private IEnumerator Dash() {
        isDashing = true;
        rb.velocity = moveDirection.normalized * stats.DashSpeed;

        Invincibility dashInvincibility = gameObject.AddComponent<Invincibility>();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Dash);

        OnDash?.Invoke();

        yield return new WaitForSeconds(stats.DashTime);

        isDashing = false;
        Destroy(dashInvincibility);
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
