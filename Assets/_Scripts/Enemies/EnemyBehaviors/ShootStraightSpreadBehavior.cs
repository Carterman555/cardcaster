using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootStraightSpreadBehavior : StraightShootBehavior {

    [SerializeField] private int bulletCount;

    public override void ShootProjectile() {
        Transform target = PlayerMeleeAttack.Instance.transform;
        Vector2 toTarget = target.position - transform.position;

        for (int i = 0; i < bulletCount; i++) {
            StraightMovement newProjectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

            float angleBetweenBullets = 15f;
            float spreadAngle = (i - (bulletCount - 1) / 2f) * angleBetweenBullets;
            Vector2 currentBulletDirection = Quaternion.Euler(0, 0, spreadAngle) * toTarget.normalized;

            newProjectile.Setup(currentBulletDirection);
            newProjectile.GetComponent<DamageOnContact>().Setup(hasStats.EnemyStats.Damage, hasStats.EnemyStats.KnockbackStrength);

            InvokeShootProjectileEvent(newProjectile.gameObject);
        }

        PlaySFX();

        InvokeShootDirectionEvent(toTarget.normalized);
        InvokeAttackEvent();
    }
}
