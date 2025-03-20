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

    public bool InDashAttackWindow => dashTimer > 0;
    private PlayerStats Stats => StatsManager.Instance.GetPlayerStats();

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();
    }

    private void OnEnable() {
        playerMovement.OnDash.AddListener(OnDash);
    }

    private void OnDisable() {
        playerMovement.OnDash.RemoveListener(OnDash);
    }

    private void OnDash() {
        dashTimer = windowToAttack;
    }

    public Collider2D[] DashAttack() {

        float angle = playerMeleeAttack.GetAttackDirection().DirectionToRotation().eulerAngles.z;
        Vector2 pos = (Vector2)transform.position + (playerMeleeAttack.GetAttackDirection() * attackSize.x * 0.5f);

        Collider2D[] cols = DamageDealer.DealCapsuleDamage(
            targetLayerMask,
            pos, attackSize, angle,
            Stats.DashDamage, Stats.KnockbackStrength);

        return cols;
    }

    private void Update() {
        dashTimer -= Time.deltaTime;
    }
}
