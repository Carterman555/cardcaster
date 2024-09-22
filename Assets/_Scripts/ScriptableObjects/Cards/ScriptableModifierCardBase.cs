using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableModifierCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributesToModify;
    public AbilityAttribute AbilityAttributesToModify => abilityAttributesToModify;

    protected virtual void ApplyToAbility() {
        
    }

    protected virtual void ApplyVisualEffect(Transform targetForVisual) {

    }
}
