using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMeleeAttack : StaticInstance<PlayerMeleeAttack>, ITargetAttacker, IHasStats {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private SlashingWeapon weapon;

    [SerializeField] private SortingGroup weaponGroup;

    private float attackTimer;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    private float GetAttackRadius() {
        float radiusMult = 1f;
        float radiusAdd = 0.5f;
        return stats.SwordSize * radiusMult + radiusAdd;
    }

    private void Update() {

        HandleAttackDirection();

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        attackTimer += Time.deltaTime;
        if (!Helpers.IsMouseOverUI() &&
            attackInput.action.triggered &&
            attackTimer > stats.AttackCooldown &&
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

        // deal damage
        Vector2 attackCenter = (Vector2)gameObject.transform.position + (GetAttackDirection() * GetAttackRadius());
        Collider2D[] targetCols = DamageDealer.DealCircleDamage(targetLayerMask, attackCenter, GetAttackRadius(), stats.Damage, stats.KnockbackStrength);

        PlayAttackFeedbacks(targetCols);
        CreateSlashEffect(GetAttackDirection());

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Swing);

        // invoke events
        OnAttack?.Invoke();

        // turn the targetCol array into health array
        Health[] targetHealths = targetCols
            ?.Select(t => t.GetComponent<Health>())
            .Where(health => health != null && !health.IsDead())
            .ToArray() ?? Array.Empty<Health>();

        foreach (Health health in targetHealths) {
            OnDamage_Target?.Invoke(health.gameObject);
        }
    }

    public void ExternalAttack(GameObject target, Vector2 attackCenter, float damageMult = 1f, float knockbackStrengthMult = 1f) {

        float damage = stats.Damage * damageMult;
        float knockbackStrength = stats.KnockbackStrength * knockbackStrengthMult;
        DamageDealer.TryDealDamage(target, transform.position, damage, knockbackStrength);

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
            Vector2 contactPos = col.ClosestPoint(transform.position);
            attackParticlesPrefab.Spawn(contactPos, Containers.Instance.Effects);
        }
    }

    private void CreateSlashEffect(Vector2 attackDirection) {
        slashPrefab.Spawn(transform.position, attackDirection.DirectionToRotation(), Containers.Instance.Effects);
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
        if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Keyboard) {
            return MouseTracker.Instance.ToMouseDirection(transform.position).normalized;
        }
        else if (InputManager.Instance.GetInputScheme() == ControlSchemeType.Controller) {
            return lastAimDirection;
        }

        Debug.LogError($"ControlSchemeType not found: {InputManager.Instance.GetInputScheme()}!");
        return Vector2.zero;
    }

    #endregion

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red; // Choose a color for the circle

            Vector2 attackCenter = (Vector2)transform.position + (GetAttackDirection() * GetAttackRadius());
            Gizmos.DrawWireSphere(attackCenter, GetAttackRadius());
        }
    }
}
