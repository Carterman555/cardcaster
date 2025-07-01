using MoreMountains.Feedbacks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMeleeAttack : StaticInstance<PlayerMeleeAttack>, ITargetAttacker {

    public event Action OnAttack;
    public static event Action OnBasicAttack;
    public event Action<GameObject> OnDamage_Target;
    public event Action<GameObject[]> OnAttack_Targets;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private SlashingWeapon weapon;

    [SerializeField] private SortingGroup weaponGroup;

    [SerializeField] private float inputBufferTime;
    private bool attackBuffered;

    private PlayerDashAttack playerDashAttack;

    private float attackTimer;

    [SerializeField] private TriggerContactTracker autoAttackTracker;

    private PlayerStats Stats => StatsManager.PlayerStats;

    private float GetAttackRadius() {
        float radiusMult = 1f;
        float radiusAdd = 0.5f;
        return Stats.SwordSize * radiusMult + radiusAdd;
    }

    protected override void Awake() {
        base.Awake();
        playerDashAttack = GetComponent<PlayerDashAttack>();
    }

    private void Update() {

        HandleAttackDirection();

        if (GameStateManager.Instance.CurrentState != GameState.Game) {
            return;
        }

        //... can attack if almost done dashing
        float dashRemainingThreshold = 0.1f;

        bool autoAttacking = SettingsManager.CurrentSettings.AutoAttack && autoAttackTracker.HasContact();
        bool attemptingAttack = attackInput.action.IsPressed() || autoAttacking;

        attackTimer += Time.deltaTime;
        if (attemptingAttack &&
            !Helpers.IsMouseOverUI() &&
            !AttackDisabled() &&
            PlayerMovement.Instance.DashingTimeRemaining < dashRemainingThreshold &&
            !attackBuffered) {

            bool cooldownReset = attackTimer > Stats.AttackCooldown;
            bool withinBufferTime = !cooldownReset && attackTimer > Stats.AttackCooldown - inputBufferTime;

            if (cooldownReset) {
                Attack();
            }
            else if (withinBufferTime) {
                attackBuffered = true;
                float timeLeftTilCooldownReset = Stats.AttackCooldown - attackTimer;
                Invoke(nameof(Attack), timeLeftTilCooldownReset);
            }
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
        attackBuffered = false;
        attackTimer = 0f;

        weapon.Swing();

        Collider2D[] targetCols;
        if (playerDashAttack.CanDashAttack(GetAttackDirection())) {
            targetCols = playerDashAttack.DashAttack();
        }
        else {
            // deal damage
            Vector2 attackCenter = (Vector2)PlayerMovement.Instance.CenterPos + (GetAttackDirection() * GetAttackRadius());
            targetCols = DamageDealer.DealCircleDamage(GameLayers.PlayerTargetLayerMask, PlayerMovement.Instance.CenterPos, attackCenter, GetAttackRadius(), Stats.BasicAttackDamage, Stats.KnockbackStrength, canCrit: true);

            slashPrefab.Spawn(PlayerMovement.Instance.CenterPos, GetAttackDirection().DirectionToRotation(), Containers.Instance.Effects);

            OnBasicAttack?.Invoke();
        }

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Swing);

        PlayAttackFeedbacks(targetCols);

        // invoke events
        OnAttack?.Invoke();

        // turn the targetCol array into health array
        EnemyHealth[] targetHealths = targetCols
            ?.Select(t => t.GetComponent<EnemyHealth>())
            .Where(health => health != null && !health.Dead)
            .ToArray() ?? Array.Empty<EnemyHealth>();

        foreach (EnemyHealth health in targetHealths) {
            OnDamage_Target?.Invoke(health.gameObject);
        }

        OnAttack_Targets?.Invoke(targetCols.Select(h => h.gameObject).ToArray());
    }

    public void ExternalAttack(GameObject target, Vector2 attackCenter, float damageMult = 1f, float knockbackStrengthMult = 1f) {

        float damage = Stats.BasicAttackDamage * damageMult;
        float knockbackStrength = Stats.KnockbackStrength * knockbackStrengthMult;
        DamageDealer.TryDealDamage(target, attackCenter, damage, knockbackStrength, canCrit: true);

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

    private Vector2 lastControllerAimDirection; // that is not Vector2.zero

    private void HandleAttackDirection() {
        Vector2 controllerAimDirection = aimSwordAction.action.ReadValue<Vector2>().normalized;
        if (controllerAimDirection != Vector2.zero) {
            lastControllerAimDirection = controllerAimDirection;
        }

        weapon.SetAttackDirection(GetAttackDirection());
    }

    // aim the sword towards the mouse or with right joystick
    public Vector2 GetAttackDirection() {

        if (SettingsManager.CurrentSettings.AutoAim) {
            Transform[] enemies = Containers.Instance.Enemies
                .GetComponentsInChildren<Transform>()
                .Skip(1) // skip Containers.Instance.Enemies
                .ToArray();

            if (enemies.Length > 0) {
                Vector2[] enemyDirections = enemies.Select(e => (Vector2)(e.transform.position - PlayerMovement.Instance.CenterPos)).ToArray();
                Vector2 closestEnemyDirection = enemyDirections.OrderBy(d => (d.x * d.x) + (d.y * d.y)).FirstOrDefault();
                return closestEnemyDirection.normalized;
            }
        }

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
            return MouseTracker.Instance.ToMouseDirection(PlayerMovement.Instance.CenterPos).normalized;
        }
        else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            return lastControllerAimDirection;
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
