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
    }
    private void OnDisable() {
        enemy.OnAttack -= AttackAnim;
    }

    private void Update() {
        anim.SetBool("move", enemy.IsMoving());
    }

    private void AttackAnim() {
        anim.SetTrigger("attack");
    }
    
    // played by anim
    private void AnimationTriggerEvent(AnimationTriggerType triggerType) {
        enemy.AnimationTriggerEvent(triggerType);
    }
}

public enum AnimationTriggerType {
    Die,
    Damaged,
    MeleeAttack,
    MeleeAttack2,
    MeleeAttack3,
    RangedAttack,
    RangedAttack2,
    RangedAttack3,
    SpawnEnemy,
}