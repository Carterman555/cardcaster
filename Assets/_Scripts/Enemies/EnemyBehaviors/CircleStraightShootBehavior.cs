using UnityEngine;

public class CircleStraightShootBehavior : EnemyBehavior {

    private IStraightProjectile projectilePrefab;
    private int projectileCount;

    private float attackTimer;

    public void Setup(IStraightProjectile projectilePrefab, int projectileCount) {
        this.projectilePrefab = projectilePrefab;
        this.projectileCount = projectileCount;
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (IsStopped()) {
            attackTimer = 0;
            return;
        }

        attackTimer += Time.deltaTime;

        float attackCooldownMult = 2f;
        float cooldown = enemy.GetStats().AttackCooldown * attackCooldownMult;
        if (attackTimer > cooldown) {
            CircleShoot();

            attackTimer = 0;
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
            GameObject projectileObject = projectilePrefab.GetObject()
                .Spawn(spawnPosition, Containers.Instance.Projectiles);
            projectileObject.GetComponent<IStraightProjectile>().Shoot(projectileDirection, enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);
            projectileObject.transform.up = projectileDirection;

            angle += angleStep;
        }
    }
}
