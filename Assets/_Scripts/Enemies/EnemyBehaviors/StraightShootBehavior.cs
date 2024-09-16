using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class StraightShootBehavior : EnemyBehavior {

    public event Action OnShoot;
    public event Action<Vector2> OnShoot_Direction;

    private StraightMovement projectilePrefab;
    private Vector2 localShootPosition;

    protected Transform target;
    private float attackTimer;

    public void Setup(StraightMovement projectilePrefab, Vector2 localShootPosition) {
        this.projectilePrefab = projectilePrefab;
        this.localShootPosition = localShootPosition;

        Stop();
    }

    public override void FrameUpdateLogic() {
        base.FrameUpdateLogic();

        if (!IsStopped()) {
            attackTimer += Time.deltaTime;
            if (attackTimer > enemy.GetStats().AttackCooldown) {
                enemy.InvokeAttack();

                attackTimer = 0;
            }
        }
        else {
            attackTimer = 0;
        }
    }

    public void StartShooting(Transform target) {
        Start();
        this.target = target;

        // less delay the first time shooting
        attackTimer = enemy.GetStats().AttackCooldown / 2f;
    }

    protected virtual void Shoot() {
        Vector2 shootPosition = (Vector2)enemy.transform.position + localShootPosition;
        StraightMovement newProjectile = projectilePrefab.Spawn(shootPosition, Containers.Instance.Projectiles);

        Vector2 toTarget = target.position - enemy.transform.position;
        newProjectile.Setup(toTarget.normalized);
        newProjectile.GetComponent<DamageOnContact>().Setup(enemy.GetStats().Damage, enemy.GetStats().KnockbackStrength);

        InvokeShoot(toTarget.normalized);
    }

    // for child behaviors to play
    protected void InvokeShoot(Vector2 direction) {
        OnShoot?.Invoke();
        OnShoot_Direction?.Invoke(direction);
    }

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);

        if (triggerType == AnimationTriggerType.ShootStraight) {
            Shoot();
        }
    }
}
