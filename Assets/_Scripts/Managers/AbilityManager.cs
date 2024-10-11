using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : StaticInstance<AbilityManager> {


    #region Abilities

    private List<ScriptableAbilityCardBase> activeAbilities = new();

    public void AddActiveAbility(ScriptableAbilityCardBase ability) {
        activeAbilities.Add(ability);
    }

    public void RemoveActiveAbility(ScriptableAbilityCardBase ability) {
        if (!activeAbilities.Contains(ability)) {
            Debug.LogError("Trying to remove ability not in list!");
            return;
        }

        activeAbilities.Remove(ability);
    }

    public bool IsAbilityActive(ScriptableAbilityCardBase ability, out ScriptableAbilityCardBase alreadyActiveAbility) {
        alreadyActiveAbility = activeAbilities.FirstOrDefault(a => a.GetType().Equals(ability.GetType()));
        return alreadyActiveAbility != null;
    }

    #endregion

    #region Modifiers

    public static event Action OnApplyModifiers;

    private List<ScriptableModifierCardBase> activeModifiers = new();

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

    #endregion
}