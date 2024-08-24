using UnityEngine;

public class SlashAttackBehavior : EnemyBehavior {

    private SlashingWeapon weapon;
    private LayerMask targetLayerMask;
    private float slashSize;

    private float attackTimer;

    private bool attacking;

    public bool IsAttacking() {
        return attacking;
    }

    public void Setup(SlashingWeapon weapon, LayerMask targetLayerMask, float slashSize) {
        this.weapon = weapon;
        this.targetLayerMask = targetLayerMask;
        this.slashSize = slashSize;

        attacking = false;

        weapon.SetTarget(Object.FindObjectOfType<PlayerMeleeAttack>().transform);
    }

    public override void FrameUpdateLogic() {

        if (attacking) {
            attackTimer += Time.deltaTime;
            if (attackTimer > enemy.GetStats().AttackCooldown) {
                attackTimer = 0;
                weapon.Swing();

                Vector2 toPlayer = PlayerMovement.Instance.transform.position - enemy.transform.position;
                DamageEnemies(toPlayer);

                enemy.InvokeAttack();
            }
        }
        else {
            attackTimer = 0;
        }
    }

    private void DamageEnemies(Vector2 attackDirection) {
        Vector2 attackCenter = (Vector2)enemy.transform.position + (attackDirection.normalized * slashSize);

        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, slashSize, targetLayerMask);
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out Health health)) {
                health.Damage(enemy.GetStats().Damage);
            }
            if (col.TryGetComponent(out Knockback knockback)) {
                Vector2 toEnemyDirection = col.transform.position - enemy.transform.position;
                knockback.ApplyKnockback(toEnemyDirection, enemy.GetStats().KnockbackStrength);
            }
        }
    }

    public void StartAttacking() {
        attacking = true;
    }
    public void StopAttacking() {
        attacking = false;
    }
}
