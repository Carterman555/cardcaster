using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootStraightSpreadBehavior : StraightShootBehavior {

    private int bulletCount;

    public ShootStraightSpreadBehavior(Enemy enemy, StraightMovement projectilePrefab, Transform shootPoint, int bulletCount) :
        base(enemy, projectilePrefab, shootPoint) {
        this.bulletCount = bulletCount;
    }

    protected override void CreateProjectile() {
        Vector2 toTarget = target.position - enemy.transform.position;

        for (int i = 0; i < bulletCount; i++) {
            StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

            float angleBetweenBullets = 15f;
            float spreadAngle = (i - (bulletCount - 1) / 2f) * angleBetweenBullets;
            Vector2 currentBulletDirection = Quaternion.Euler(0, 0, spreadAngle) * toTarget.normalized;

            newProjectile.Setup(currentBulletDirection);
            newProjectile.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);
        }

        InvokeShoot(toTarget.normalized);
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        if (triggerType == AnimationTriggerType.ShootStraightSpread) {
            CreateProjectile();
        }
    }
}
