using UnityEngine;
using System;

public class SwordSlashBehavior : MonoBehaviour, IAttacker {

    public event Action OnAttack;

    private IHasEnemyStats hasStats;

    [SerializeField] private SlashingWeapon weapon;
    [SerializeField] private float slashSize;

    private float attackTimer;

    private void Awake() {
        hasStats = GetComponent<IHasEnemyStats>();
    }

    private void OnEnable() {
        attackTimer = 0;
    }

    private void Update() {
        attackTimer += Time.deltaTime;
        if (attackTimer > hasStats.EnemyStats.AttackCooldown) {
            attackTimer = 0;
            Slash();
        }
    }

    private void Slash() {
        weapon.Swing();

        // deal damage
        Vector2 toPlayer = PlayerMovement.Instance.CenterPos - transform.position;
        Vector2 attackCenter = (Vector2)transform.position + (toPlayer.normalized * slashSize);
        DamageDealer.DealCircleDamage(GameLayers.PlayerLayerMask, transform.position, attackCenter, slashSize, hasStats.EnemyStats.Damage, hasStats.EnemyStats.KnockbackStrength);

        OnAttack?.Invoke();
    }
}
