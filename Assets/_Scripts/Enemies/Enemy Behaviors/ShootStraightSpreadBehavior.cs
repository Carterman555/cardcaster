using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootStraightSpreadBehavior : StraightShootBehavior {


    private IStraightProjectile projectile;
    private Vector2 localShootPosition;
    private int bulletCount;

    public void Setup(IStraightProjectile projectile, Vector2 localShootPosition, int bulletCount) {
        this.projectile = projectile;
        this.localShootPosition = localShootPosition;
        this.bulletCount = bulletCount;
    }

    protected override void Shoot() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        Vector2 toTarget = target.position - enemy.transform.position;

        for (int i = 0; i < bulletCount; i++) {
            IStraightProjectile newProjectile = projectile.GetObject().Spawn(shootPosition, Containers.Instance.Projectiles).GetComponent<IStraightProjectile>();

            float angleBetweenBullets = 15f;
            float spreadAngle = (i - (bulletCount - 1) / 2f) * angleBetweenBullets;
            Vector2 currentBulletDirection = Quaternion.Euler(0, 0, spreadAngle) * toTarget.normalized;

            newProjectile.Shoot(currentBulletDirection, enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);
        }

        InvokeShoot(toTarget.normalized);
    }
}
