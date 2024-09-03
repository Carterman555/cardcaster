using UnityEngine;

public class EnemySlashAttackBehavior : EnemyBehavior {

    private SlashAttackBehavior slashAttackBehavior;

    private float attackTimer;
    private float slashSize;

    public void Setup(SlashingWeapon weapon, LayerMask targetLayerMask, float slashSize) {
        this.slashSize = slashSize;

        slashAttackBehavior = new();
        slashAttackBehavior.Initialize(enemy.gameObject, enemy);
        slashAttackBehavior.Setup(weapon, targetLayerMask);
        Stop();

        weapon.SetTarget(Object.FindObjectOfType<PlayerMeleeAttack>().transform);
    }

    public override void FrameUpdateLogic() {

        if (!IsStopped()) {
            attackTimer += Time.deltaTime;
            if (attackTimer > enemy.GetStats().AttackCooldown) {
                attackTimer = 0;

                Vector2 toPlayer = PlayerMovement.Instance.transform.position - enemy.transform.position;
                slashAttackBehavior.Swing(toPlayer, slashSize);

                enemy.InvokeAttack();
            }
        }
        else {
            attackTimer = 0;
        }
    }
}
