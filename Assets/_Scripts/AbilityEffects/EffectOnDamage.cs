using System;
using UnityEngine;

public class EffectOnDamage {

    private Action<GameObject> onAttackAction;
    private ITargetAttacker[] attackers;

    public EffectOnDamage(Action<GameObject> onAttackAction, Transform attackerTransform, ParticleSystem particlesPrefab) {
        this.onAttackAction = onAttackAction;

        attackers = attackerTransform.GetComponentsInChildren<ITargetAttacker>();
        SubToAttackEvents();
    }

    public void Disable() {
        UnsubFromAttackEvents();
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
