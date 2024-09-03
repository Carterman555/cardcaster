using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ShockwaveBehavior : EnemyBehavior {

    private IStraightProjectile shockwavePrefab;
    private int shockwaveCount;

    private float attackTimer;

    public void Setup(IStraightProjectile shockwavePrefab, int shockwaveCount) {
        this.shockwavePrefab = shockwavePrefab;
        this.shockwaveCount = shockwaveCount;
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
            CreateShockwaves();

            attackTimer = 0;
        }
    }

    private void CreateShockwaves() {

        // Calculate the angle between each shockwave
        float angleStep = 360f / shockwaveCount;
        float angle = 0f;

        for (int i = 0; i < shockwaveCount; i++) {
            Vector2 shockwaveDirection = angle.RotationToDirection();

            float distanceFromCenter = 1f;

            Vector2 spawnPosition = (Vector2)enemy.transform.position + shockwaveDirection * distanceFromCenter;
            GameObject shockwaveObject = shockwavePrefab.GetObject()
                .Spawn(spawnPosition, Containers.Instance.Projectiles);
            shockwaveObject.GetComponent<IStraightProjectile>().Shoot(shockwaveDirection, enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);
            shockwaveObject.transform.up = shockwaveDirection;

            angle += angleStep;
        }
    }
}
