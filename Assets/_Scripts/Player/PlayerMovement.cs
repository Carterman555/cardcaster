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

        facingRight = true;
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
    }

    private void Update() {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            anim.SetBool("move", false);
            return;
        }

        if (InputDisabled()) {
            return;
        }

        if (IsStopped()) {
            anim.SetBool("move", false);
            return;
        }

        bool moving = rb.velocity.magnitude > 0;

        anim.SetBool("move", moving);

        moveDirection = moveInput.action.ReadValue<Vector2>();

        if (dashAction.action.triggered && !isDashing && !knockback.IsApplyingKnockback() && moveDirection.magnitude > 0f) {
            StartCoroutine(Dash());
        }

        FaceAttackDirection();
    }

    private void FixedUpdate() {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (InputDisabled()) {
            return;
        }

        if (IsStopped()) {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!isDashing && !knockback.IsApplyingKnockback()) {
            rb.velocity = moveDirection * stats.MoveSpeed;
        }
    }

    #region Dash

    public UnityEvent OnDash;

    [Header("Dash")]
    [SerializeField] private InputActionReference dashAction;

    private FadeEffect dashFade;

    private bool isDashing;

    private IEnumerator Dash() {

        isDashing = true;
        rb.velocity = moveDirection.normalized * stats.DashSpeed;
        Invincibility dashInvincibility = gameObject.AddComponent<Invincibility>();

        dashFade = PlayerVisual.Instance.AddFadeEffect(0, 0.5f, 0.1f);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Dash);

        OnDash?.Invoke();

        yield return new WaitForSeconds(stats.DashTime);

        isDashing = false;

        Destroy(dashInvincibility);

        PlayerVisual.Instance.RemoveFadeEffect(dashFade, 0.1f);
    }

    #endregion

    #region Face Towards Attack Direction

    public event Action<bool> OnChangedFacing; // bool: facing right

    private bool facingRight;

    private void FaceAttackDirection() {

        float attackXPos = PlayerMeleeAttack.Instance.GetAttackDirection().x;

        bool shouldFaceRight = attackXPos > 0;

        if (!facingRight && shouldFaceRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            facingRight = true;
            OnChangedFacing?.Invoke(facingRight);
        }
        else if (facingRight && !shouldFaceRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
            facingRight = false;
            OnChangedFacing?.Invoke(facingRight);
        }
    }

    #endregion

    #region Stop Movement

    private bool IsStopped() {
        return TryGetComponent(out StopMovement stopMovement);
    }

    // add stopmovement component instead of just having a bool, so that when the movement is stopped in different ways
    // at the same time, they don't interfere with eachother
    public void StopMovement() {
        gameObject.AddComponent<StopMovement>();
    }

    public void AllowMovement() {
        if (TryGetComponent(out StopMovement stopMovement)) {
            Destroy(stopMovement);
        }
        else {
            Debug.LogWarning("Tried allowing movement when already allowed!");
        }
    }

    private bool InputDisabled() {
        return TryGetComponent(out DisableMoveInput disableInput);
    }

    public void DisableMoveInput() {
        gameObject.AddComponent<DisableMoveInput>();
    }

    public void AllowMoveInput() {
        if (TryGetComponent(out DisableMoveInput disableInput)) {
            Destroy(disableInput);
        }
        else {
            Debug.LogWarning("Tried allowing move input when already allowed!");
        }
    }

    #endregion
}