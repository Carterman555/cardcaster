using MoreMountains.Feedbacks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : StaticInstance<PlayerMeleeAttack>, ITargetAttacker, IHasStats {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private SlashingWeapon weapon;

    private float attackTimer;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    private float GetAttackRadius() {
        float radiusMult = 0.65f;
        float radiusAdd = 0.65f;
        return stats.SwordSize * radiusMult + radiusAdd;
    }

    [SerializeField] private SpriteRenderer hand;

    private void Start() {
        weapon.SetTarget(MouseTracker.Instance.transform);
    }

    private void Update() {

        if (GameStateManager.Instance.GetCurrentState() != GameState.Game) {
            return;
        }

        attackTimer += Time.deltaTime;
        //if (attackInput.action.triggered && attackTimer > stats.AttackCooldown) {
        if (Input.GetMouseButtonDown(0) && attackTimer > stats.AttackCooldown) {
            Attack();
            attackTimer = 0f;
        }

        // hide sword and hand behind player head
        if (weapon.InUpPos()) {
            ReferenceSystem.Instance.PlayerSwordVisual.sortingOrder = 0;
            hand.sortingOrder = 0;
        }
        else {
            ReferenceSystem.Instance.PlayerSwordVisual.sortingOrder = 1;
            hand.sortingOrder = 1;
        }
    }

    private void Attack() {
        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(transform.position);

        weapon.Swing();

        // deal damage
        Vector2 attackCenter = (Vector2)gameObject.transform.position + (toMouseDirection.normalized * GetAttackRadius());
        Collider2D[] targetCols = DamageDealer.DealCircleDamage(targetLayerMask, attackCenter, GetAttackRadius(), stats.Damage, stats.KnockbackStrength);

        PlayAttackFeedbacks(targetCols);
        CreateSlashEffect(toMouseDirection);

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


    #region Effects and Feedbacks

    [Header("Effects and Feedbacks")]
    [SerializeField] private MMF_Player hitFeedbacks;
    [SerializeField] private ParticleSystem attackParticlesPrefab;
    [SerializeField] private Transform slashPrefab;

    private void PlayAttackFeedbacks(Collider2D[] targetCols) {

        // play feedbacks if hit something
        if (targetCols.Length > 0) {
            hitFeedbacks.PlayFeedbacks();
        }

        foreach (Collider2D col in targetCols) {
            Vector2 contactPos = col.ClosestPoint(transform.position);
            attackParticlesPrefab.Spawn(contactPos, Containers.Instance.Effects);
        }
    }

    private void CreateSlashEffect(Vector2 toMouseDirection) {
        slashPrefab.Spawn(transform.position, toMouseDirection.DirectionToRotation(), Containers.Instance.Effects);

    }

    #endregion


    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red; // Choose a color for the circle

            Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(transform.position);
            Vector2 attackCenter = (Vector2)transform.position + (toMouseDirection * GetAttackRadius());
            Gizmos.DrawWireSphere(attackCenter, GetAttackRadius());
        }
    }


}
