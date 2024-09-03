using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMeleeAttack : MonoBehaviour, ICanAttack, IHasStats {

    public event Action OnAttack;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform slashPrefab;
    [SerializeField] private LayerMask targetLayerMask;

    [SerializeField] private SlashingWeapon weapon;
    private SlashAttackBehavior slashAttackBehavior;

    private float attackTimer;

    private PlayerStats stats => StatsManager.Instance.GetPlayerStats();
    public Stats GetStats() => stats;

    private void Start() {
        weapon.SetTarget(MouseTracker.Instance.transform);
        slashAttackBehavior = new();
        slashAttackBehavior.Initialize(gameObject, this);
        slashAttackBehavior.Setup(weapon, targetLayerMask);
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
        slashAttackBehavior.Swing(toMouseDirection, stats.SwordSize);
        CreateEffect(toMouseDirection);

        OnAttack?.Invoke();
    }

    private void CreateEffect(Vector2 toMouseDirection) {
        slashPrefab.Spawn(transform.position, toMouseDirection.DirectionToRotation(), Containers.Instance.Effects);
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
