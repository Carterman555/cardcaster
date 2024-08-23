using UnityEngine;

public class SlashAttackBehavior : EnemyBehavior {

    private SlashingWeapon weapon;
    private LayerMask targetLayerMask;
    private float slashSize;
    private float attackCooldown;
    private float damage;

    private float attackTimer;

    private bool attacking;

    public bool IsAttacking() {
        return attacking;
    }

    public void Setup(SlashingWeapon weapon, LayerMask targetLayerMask, float slashSize, float attackCooldown, float damage) {
        this.weapon = weapon;
        this.targetLayerMask = targetLayerMask;
        this.slashSize = slashSize;
        this.attackCooldown = attackCooldown;
        this.damage = damage;

        attacking = false;

        weapon.SetTarget(Object.FindObjectOfType<PlayerMeleeAttack>().transform);
    }

    public override void FrameUpdateLogic() {

        if (attacking) {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackCooldown) {
                attackTimer = 0;
                weapon.Swing();

                enemy.InvokeAttack();
            }
        }
        else {
            attackTimer = 0;
        }
    }

    private void DamageEnemies(Vector2 toMouseDirection) {
        Vector2 attackCenter = (Vector2)enemy.transform.position + (toMouseDirection * slashSize);

        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, slashSize, targetLayerMask);
        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out Health health)) {
                health.Damage(damage);
            }
            //if (col.TryGetComponent(out Knockback knockback)) {
            //    Vector2 toEnemyDirection = col.transform.position - transform.position;
            //    knockback.ApplyKnockback(toEnemyDirection, stats.KnockbackStrength);
            //}
        }
    }

    public void StartAttacking() {
        attacking = true;
    }
    public void StopAttacking() {
        attacking = false;
    }
}
