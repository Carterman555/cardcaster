using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMeleeAttack : StaticInstance<PlayerMeleeAttack>, ITargetAttacker, IHasCommonStats {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private SlashingWeapon weapon;

    [SerializeField] private SortingGroup weaponGroup;

    private PlayerDashAttack playerDashAttack;

    private float attackTimer;

    public PlayerStats PlayerStats => StatsManager.Instance.GetPlayerStats();
    public CommonStats CommonStats => PlayerStats.CommonStats;

    private float GetAttackRadius() {
        float radiusMult = 1f;
        float radiusAdd = 0.5f;
        return PlayerStats.SwordSize * radiusMult + radiusAdd;
    }

    protected override void Awake() {
        base.Awake();
        playerDashAttack = GetComponent<PlayerDashAttack>();
    }

    private void Update() {

        HandleAttackDirection();

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        attackTimer += Time.deltaTime;
        if (!Helpers.IsMouseOverUI() &&
            attackInput.action.triggered &&
            attackTimer > PlayerStats.CommonStats.AttackCooldown &&
            !AttackDisabled()) {

            Attack();
            attackTimer = 0f;
        }

        // hide sword and hand behind player head
        if (weapon.InUpPos()) {
            weaponGroup.sortingOrder = -1;
        }
        else {
            weaponGroup.sortingOrder = 1;
        }
    }

    private void Attack() {

        weapon.Swing();

        Collider2D[] targetCols;
        if (playerDashAttack.GetCanDashAttack(GetAttackDirection())) {
            targetCols = playerDashAttack.DashAttack();
        }
        else {
            // deal damage
            Vector2 attackCenter = (Vector2)PlayerMovement.Instance.CenterPos + (GetAttackDirection() * GetAttackRadius());
            targetCols = DamageDealer.DealCircleDamage(targetLayerMask, attackCenter, GetAttackRadius(), PlayerStats.CommonStats.Damage, PlayerStats.CommonStats.KnockbackStrength);

            slashPrefab.Spawn(PlayerMovement.Instance.CenterPos, GetAttackDirection().DirectionToRotation(), Containers.Instance.Effects);

            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Swing);
        }

        PlayAttackFeedbacks(targetCols);

        // invoke events
        OnAttack?.Invoke();

        // turn the targetCol array into health array
        Health[] targetHealths = targetCols
            ?.Select(t => t.GetComponent<Health>())
            .Where(health => health != null && !health.Dead)
            .ToArray() ?? Array.Empty<Health>();

        foreach (Health health in targetHealths) {
            OnDamage_Target?.Invoke(health.gameObject);
        }
    }

    public void ExternalAttack(GameObject target, Vector2 attackCenter, float damageMult = 1f, float knockbackStrengthMult = 1f) {

        float damage = PlayerStats.CommonStats.Damage * damageMult;
        float knockbackStrength = PlayerStats.CommonStats.KnockbackStrength * knockbackStrengthMult;
        DamageDealer.TryDealDamage(target, attackCenter, damage, knockbackStrength);

        // invoke events
        OnAttack?.Invoke();
        OnDamage_Target?.Invoke(target);
    }


    #region Disable Attack

    private bool AttackDisabled() {
        return TryGetComponent(out DisableAttack disableAttack);
    }

    // add DisableAttack component instead of just having a bool, so that when the attack is stopped in different ways
    // at the same time, they don't interfere with eachother
    public void DisableAttack() {
        gameObject.AddComponent<DisableAttack>();
    }

    public void AllowAttack() {
        if (TryGetComponent(out DisableAttack disableAttack)) {
            Destroy(disableAttack);
        }
        else {
            Debug.LogWarning("Tried allowing attack when already allowed!");
        }
    }

    #endregion

    #region Effects and Feedbacks

    [Header("Effects and Feedbacks")]
    [SerializeField] private MMF_Player attackFeedbacks;
    [SerializeField] private MMF_Player hitFeedbacks;
    [SerializeField] private ParticleSystem attackParticlesPrefab;
    [SerializeField] private Transform slashPrefab;

    private void PlayAttackFeedbacks(Collider2D[] targetCols) {

        attackFeedbacks.PlayFeedbacks();

        // play feedbacks if hit something
        if (targetCols.Length > 0) {
            hitFeedbacks.PlayFeedbacks();
        }

        foreach (Collider2D col in targetCols) {
            Vector2 contactPos = col.ClosestPoint(PlayerMovement.Instance.CenterPos);
            attackParticlesPrefab.Spawn(contactPos, Containers.Instance.Effects);
        }
    }

    #endregion

    #region Attack Direction

    [SerializeField] private InputActionReference aimSwordAction;

    private Vector2 lastAimDirection; // that is not Vector2.zero

    private void HandleAttackDirection() {
        Vector2 aimDirection = aimSwordAction.action.ReadValue<Vector2>().normalized;
        if (aimDirection != Vector2.zero) {
            lastAimDirection = aimDirection;
        }

        weapon.SetAttackDirection(GetAttackDirection());
    }

    // aim the sword towards the mouse or with right joystick
    public Vector2 GetAttackDirection() {
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
            return MouseTracker.Instance.ToMouseDirection(PlayerMovement.Instance.CenterPos).normalized;
        }
        else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            return lastAimDirection;
        }

        Debug.LogError($"ControlSchemeType not found: {InputManager.Instance.GetControlScheme()}!");
        return Vector2.zero;
    }

    #endregion

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red; // Choose a color for the circle

            Vector2 attackCenter = (Vector2)PlayerMovement.Instance.CenterPos + (GetAttackDirection() * GetAttackRadius());
            Gizmos.DrawWireSphere(attackCenter, GetAttackRadius());
        }
    }
}
