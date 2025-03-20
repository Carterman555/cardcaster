using UnityEngine;
using System;

public class SwordSlashBehavior : MonoBehaviour, IAttacker {

    public event Action OnAttack;

    private IHasStats hasStats;

    [SerializeField] private SlashingWeapon weapon;
    [SerializeField] private float slashSize;

    private float attackTimer;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();
    }

    private void OnEnable() {
        attackTimer = 0;
    }

    private void Update() {
        attackTimer += Time.deltaTime;
        if (attackTimer > hasStats.Stats.AttackCooldown) {
            attackTimer = 0;
            Slash();
        }
    }

    private void Slash() {
        weapon.Swing();

        // deal damage
        Vector2 toPlayer = PlayerMovement.Instance.CenterPos - transform.position;
        Vector2 attackCenter = (Vector2)transform.position + (toPlayer.normalized * slashSize);
        DamageDealer.DealCircleDamage(GameLayers.PlayerLayerMask, attackCenter, slashSize, hasStats.Stats.Damage, hasStats.Stats.KnockbackStrength);

        OnAttack?.Invoke();
    }
}
