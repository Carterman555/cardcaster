using UnityEngine;

public class CircleStraightShootBehavior : EnemyBehavior {

    private StraightMovement projectilePrefab;
    private int projectileCount;
    private bool specialAttack;
    private float attackCooldownMult;
    private TimedActionBehavior timedActionBehavior;

    public CircleStraightShootBehavior(Enemy enemy, StraightMovement projectilePrefab, int projectileCount, bool specialAttack = false, float attackCooldownMult = 1f) : base(enemy) {
        this.projectilePrefab = projectilePrefab;
        this.projectileCount = projectileCount;
        this.specialAttack = specialAttack;
        this.attackCooldownMult = attackCooldownMult;

        timedActionBehavior = new TimedActionBehavior(
            enemy.GetStats().AttackCooldown * attackCooldownMult,
            () => {
                if (specialAttack) {
                    enemy.InvokeSpecialAttack();
                }
                else {
                    enemy.InvokeAttack();
                }
            }
        );

        Stop();
    }

    public override void Start() {
        base.Start();
        timedActionBehavior.Start();
    }

    public override void Stop() {
        base.Stop();
        timedActionBehavior.Stop();
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();
        if (!IsStopped()) {
            timedActionBehavior.UpdateLogic();
        }
    }

    private void CircleShoot() {
        // Calculate the angle between each shockwave
        float angleStep = 360f / projectileCount;
        float angle = 0f;
        for (int i = 0; i < projectileCount; i++) {
            Vector2 projectileDirection = angle.RotationToDirection();

            float distanceFromCenter = 1f;

            Vector2 spawnPosition = (Vector2)enemy.transform.position + projectileDirection * distanceFromCenter;
            StraightMovement projectile = projectilePrefab
                .Spawn(spawnPosition, Containers.Instance.Projectiles);
            projectile.Setup(projectileDirection);
            projectile.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);
            projectile.transform.up = projectileDirection;

            angle += angleStep;
        }
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);
        if (triggerType == AnimationTriggerType.CircleStraightShoot) {
            CircleShoot();
        }
    }
}