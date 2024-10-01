using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : StaticInstance<AbilityManager> {

    public static event Action OnApplyModifiers;

    private List<ScriptableModifierCardBase> activeModifiers = new List<ScriptableModifierCardBase>();

    public int ActiveModifierCount() {
        return activeModifiers.Count; 
    }

    public bool IsModifierActive(ScriptableModifierCardBase modifier) {
        bool modifierActive = activeModifiers.Any(m => m.CardType == modifier.CardType);
        return modifierActive;
    }

    public void AddModifier(ScriptableModifierCardBase modifier) {
        if (modifier.CanStackWithSelf || !IsModifierActive(modifier)) {
            activeModifiers.Add(modifier);
        }
    }

    public void ApplyModifiers(ScriptableAbilityCardBase card) {
        foreach (var modifier in activeModifiers) {
            if (card.IsCompatible(modifier)) {
                modifier.ApplyToAbility(card);
            }
        }
        activeModifiers.Clear();

        OnApplyModifiers?.Invoke();
    }
}