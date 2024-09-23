using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : StaticInstance<AbilityManager> {

    private List<ScriptableModifierCardBase> activeModifiers = new List<ScriptableModifierCardBase>();

    public void AddModifier(ScriptableModifierCardBase modifier) {
        activeModifiers.Add(modifier);
    }

    public void ApplyModifiers(ScriptableAbilityCardBase card) {
        foreach (var modifier in activeModifiers) {
            if (card.IsModifiable && card.IsCompatible(modifier)) {
                modifier.ApplyToAbility(card);
            }
        }
        activeModifiers.Clear();
    }
}