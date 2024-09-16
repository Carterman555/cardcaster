using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {

    [SerializeField] private Enemy enemy;
    private Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void OnEnable() {
        enemy.OnAttack += AttackAnim;
        enemy.OnSpecialAttack += SpecialAttackAnim;
    }
    private void OnDisable() {
        enemy.OnAttack -= AttackAnim;
        enemy.OnSpecialAttack -= SpecialAttackAnim;
    }

    private void Update() {
        anim.SetBool("move", enemy.IsMoving());
    }

    private void AttackAnim() {
        anim.SetTrigger("attack");
    }

    private void SpecialAttackAnim() {
        anim.SetTrigger("specialAttack");
    }

    // played by anim
    private void AnimationTriggerEvent(AnimationTriggerType triggerType) {
        enemy.AnimationTriggerEvent(triggerType);
    }
}

public enum AnimationTriggerType {
    Die,
    Damaged,
    ShootTargetProjectile,
    SpawnEnemy,
    CircleStraightShoot,
    CircleSlash,
    Explode,
    ShootStraight,
    ShootStraightSpread,
    MeleeAttack,
}