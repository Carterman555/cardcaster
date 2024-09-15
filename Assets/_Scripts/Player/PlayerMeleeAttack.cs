using MoreMountains.Feedbacks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : StaticInstance<PlayerMeleeAttack>, IAttacker, IHasStats {

    public event Action OnAttack;
    public static event Action<Vector2> OnAttack_Position;
    public static event Action<Health[]> OnAttack_Targets;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private SlashingWeapon weapon;

    private float attackTimer;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    [SerializeField] private Animator anim;

    private void Start() {
        weapon.SetTarget(MouseTracker.Instance.transform);
    }

    private void Update() {
        //if (attackInput.action.triggered) {

        attackTimer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && attackTimer > stats.AttackCooldown) {
            Attack();
            attackTimer = 0f;
        }
    }

    private void Attack() {
        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(transform.position);

        weapon.Swing();

        // deal damage
        Vector2 attackCenter = (Vector2)gameObject.transform.position + (toMouseDirection.normalized * stats.SwordSize);
        Collider2D[] targetCols = CircleDamage.DealDamage(targetLayerMask, attackCenter, stats.SwordSize, stats.Damage, stats.KnockbackStrength);

        PlayAttackFeedbacks(targetCols);
        CreateSlashEffect(toMouseDirection);

        // invoke events
        OnAttack?.Invoke();
        OnAttack_Position?.Invoke(attackCenter);

        // turn the targetCol array into health array
        Health[] targetHealths = targetCols
            ?.Select(t => t.GetComponent<Health>())
            .Where(health => health != null && !health.IsDead())
            .ToArray() ?? Array.Empty<Health>();

        OnAttack_Targets?.Invoke(targetHealths);
    }


    #region Effects and Feedbacks

    [Header("Effects and Feedbacks")]
    [SerializeField] private MMF_Player hitFeedbacks;
    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private Transform slashPrefab;

    private void PlayAttackFeedbacks(Collider2D[] targetCols) {

        anim.SetTrigger("AttackTrigger");

        // play feedbacks if hit something
        if (targetCols.Length > 0) {
            hitFeedbacks.PlayFeedbacks();
        }

        foreach (Collider2D col in targetCols) {
            attackParticles.Spawn(col.transform.position, Containers.Instance.Effects);
        }
    }

    private void CreateSlashEffect(Vector2 toMouseDirection) {
        slashPrefab.Spawn(transform.position, toMouseDirection.DirectionToRotation(), Containers.Instance.Effects);

    }

    [SerializeField] private SpriteRenderer swordVisual;

    public SpriteRenderer GetSwordVisual() {
        return swordVisual;
    }

    #endregion


    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red; // Choose a color for the circle

            Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(transform.position);
            Vector2 attackCenter = (Vector2)transform.position + (toMouseDirection * stats.SwordSize);
            Gizmos.DrawWireSphere(attackCenter, stats.SwordSize);
        }
    }


}
