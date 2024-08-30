using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour, ICanAttack {

    public event Action OnAttack;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform slashPrefab;
    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private SlashingWeapon weapon;

    private float attackTimer;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();

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

        CreateEffect(toMouseDirection);
        DamageEnemies(toMouseDirection);
        weapon.Swing();

        OnAttack?.Invoke();
    }

    private void CreateEffect(Vector2 toMouseDirection) {
        slashPrefab.Spawn(transform.position, toMouseDirection.DirectionToRotation(), Containers.Instance.Effects);

    }

    private void DamageEnemies(Vector2 toMouseDirection) {
        Vector2 attackCenter = (Vector2)transform.position + (toMouseDirection * stats.SwordSize);

        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, stats.SwordSize, targetLayerMask);
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out IDamagable damagable)) {
                damagable.Damage(stats.Damage);
            }
            if (col.TryGetComponent(out Knockback knockback)) {
                Vector2 toEnemyDirection = col.transform.position - transform.position;
                knockback.ApplyKnockback(toEnemyDirection, stats.KnockbackStrength);
            }
        }
    }

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red; // Choose a color for the circle

            Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(transform.position);
            Vector2 attackCenter = (Vector2)transform.position + (toMouseDirection * stats.SwordSize);
            Gizmos.DrawWireSphere(attackCenter, stats.SwordSize);
        }
    }
}
