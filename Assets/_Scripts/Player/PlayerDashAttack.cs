using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDashAttack : MonoBehaviour {

    private PlayerMovement playerMovement;
    private PlayerMeleeAttack playerMeleeAttack;

    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private Vector2 attackSize;
    [SerializeField] private float windowToAttack = 0.5f;
    private float dashTimer;

    private Vector2 dashDirection;

    [SerializeField] private Transform slashPrefab;

    

    private PlayerStats Stats => StatsManager.Instance.GetPlayerStats();

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();
    }

    private void OnEnable() {
        playerMovement.OnDash_Direction += OnDash;
    }

    private void OnDisable() {
        playerMovement.OnDash_Direction -= OnDash;
    }

    private void OnDash(Vector2 dashDirection) {
        this.dashDirection = dashDirection;
        dashTimer = windowToAttack;
    }

    public Collider2D[] DashAttack() {

        Vector2 attackDirection = playerMeleeAttack.GetAttackDirection();

        float angle = attackDirection.DirectionToRotation().eulerAngles.z;
        Vector2 pos = (Vector2)playerMovement.CenterPos + (playerMeleeAttack.GetAttackDirection() * attackSize.x * 0.5f);

        Collider2D[] cols = DamageDealer.DealCapsuleDamage(
            targetLayerMask,
            pos, attackSize, angle,
            Stats.DashAttackDamage, Stats.CommonStats.KnockbackStrength);

        slashPrefab.Spawn(playerMovement.CenterPos, attackDirection.DirectionToRotation(), Containers.Instance.Effects);

        return cols;
    }

    public bool GetCanDashAttack(Vector2 attackDirection) {
        bool withinDashWindow = dashTimer > 0;
        bool directionsMatch = DirectionsWithinRange(dashDirection, attackDirection, maxAngle: 60f);
        return withinDashWindow && directionsMatch;
    }

    private bool DirectionsWithinRange(Vector3 dir1, Vector3 dir2, float maxAngle) {
        dir1.Normalize();
        dir2.Normalize();

        float dot = Vector3.Dot(dir1, dir2);
        float threshold = Mathf.Cos(maxAngle * Mathf.Deg2Rad);

        return dot >= threshold;
    }

    private void Update() {
        dashTimer -= Time.deltaTime;
    }
}
