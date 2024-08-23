using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightShootBehavior : EnemyBehavior {

    private IStraightProjectile projectile;
    private Vector2 localShootPosition;

    private Transform target;
    private float attackTimer;

    private bool shooting;

    public void Setup(IStraightProjectile projectile, Vector2 localShootPosition) {
        this.projectile = projectile;
        this.localShootPosition = localShootPosition;
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (shooting) {
            attackTimer += Time.deltaTime;
            if (attackTimer > enemy.GetStats().AttackCooldown) {
                Shoot();
                attackTimer = 0;
            }
        }
        else {
            attackTimer = 0;
        }
    }

    private void Shoot() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        IStraightProjectile newProjectile = projectile.GetObject().Spawn(shootPosition, Containers.Instance.Projectiles).GetComponent<IStraightProjectile>();
        newProjectile.GetObject().transform.localScale = projectile.GetObject().transform.localScale; // reset scale

        Vector2 toTarget = target.position - enemy.transform.position;
        newProjectile.Shoot(toTarget, enemy.GetStats().Damage);
    }

    public void StartShooting(Transform target) {
        shooting = true;

        this.target = target;
    }

    public void StopShooting() {
        shooting = false;
    }

}
