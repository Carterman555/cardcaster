using UnityEngine;
using System;

public class SwordSlashBehavior : MonoBehaviour, IAttacker {

    public event Action OnAttack;

    private IHasStats hasStats;

    private SlashingWeapon weapon;
    private float slashSize;

    private float attackTimer;

    private void Awake() {
        hasStats = GetComponent<IHasStats>();

        weapon.SetTarget(Object.FindObjectOfType<PlayerMeleeAttack>().transform);
    }

    private void OnEnable() {
        attackTimer = 0;
    }

    protected SwordSlashBehavior(Enemy enemy, SlashingWeapon weapon, LayerMask targetLayerMask, float slashSize) : base(enemy) {
        this.weapon = weapon;
        this.targetLayerMask = targetLayerMask;
        this.slashSize = slashSize;

        Stop();

    }


    private void Update() {
        attackTimer += Time.deltaTime;
        if (attackTimer > hasStats.GetStats().AttackCooldown) {
            attackTimer = 0;
            Slash();
        }
    }

    private void Slash() {
        weapon.Swing();

        // deal damage
        Vector2 toPlayer = PlayerMovement.Instance.transform.position - transform.position;
        Vector2 attackCenter = (Vector2)transform.position + (toPlayer.normalized * slashSize);
        DamageDealer.DealCircleDamage(GameLayers.PlayerLayerMask, attackCenter, slashSize, hasStats.GetStats().Damage, hasStats.GetStats().KnockbackStrength);

        OnAttack?.Invoke();
    }
}
