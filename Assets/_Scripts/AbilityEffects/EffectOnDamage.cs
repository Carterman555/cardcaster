using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOnDamage {

    private Action<GameObject> onAttackAction;
    private ITargetAttacker[] attackers;

    private ParticleSystem particles;


    public EffectOnDamage(Action<GameObject> onAttackAction, Transform attackerTransform, ParticleSystem particlesPrefab) {
        this.onAttackAction = onAttackAction;

        attackers = attackerTransform.GetComponentsInChildren<ITargetAttacker>();
        SubToAttackEvents();

        // parent to biggest renderer in order to match the transform to match the sprite shape and make sure the
        // particles emit from the visual and move with it
        Transform parent = attackerTransform.GetBiggestRenderer().transform;
        particles = particlesPrefab.Spawn(parent);
    }

    public void Disable() {
        UnsubFromAttackEvents();
        particles.gameObject.TryReturnToPool();
    }

    private void SubToAttackEvents() {
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target += onAttackAction;
        }
    }

    private void UnsubFromAttackEvents() {
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target -= onAttackAction;
        }
    }

    
}
