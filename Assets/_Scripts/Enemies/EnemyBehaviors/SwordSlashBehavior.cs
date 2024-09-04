using UnityEngine;

public class SwordSlashBehavior : EnemyBehavior {

    private SlashingWeapon weapon;
    private LayerMask targetLayerMask;
    private float slashSize;

    private float attackTimer;

    public void Setup(SlashingWeapon weapon, LayerMask targetLayerMask, float slashSize) {
        this.weapon = weapon;
        this.targetLayerMask = targetLayerMask;
        this.slashSize = slashSize;

        Stop();

        weapon.SetTarget(Object.FindObjectOfType<PlayerMeleeAttack>().transform);
    }

    public override void FrameUpdateLogic() {

        if (!IsStopped()) {
            attackTimer += Time.deltaTime;
            if (attackTimer > enemy.GetStats().AttackCooldown) {
                attackTimer = 0;

                weapon.Swing();

                // deal damage
                Vector2 toPlayer = PlayerMovement.Instance.transform.position - enemy.transform.position;
                Vector2 attackCenter = (Vector2)enemy.transform.position + (toPlayer.normalized * slashSize);
                CircleDamage.DealDamage(targetLayerMask, attackCenter, slashSize, enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

                enemy.InvokeAttack();
            }
        }
        else {
            attackTimer = 0;
        }
    }
}
