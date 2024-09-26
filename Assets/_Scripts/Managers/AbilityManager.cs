using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : StaticInstance<AbilityManager> {

    private List<ScriptableModifierCardBase> activeModifiers = new List<ScriptableModifierCardBase>();

    public void AddModifier(ScriptableModifierCardBase modifier) {
        bool modifierAlreadyActive = activeModifiers.Any(m => m.CardType == modifier.CardType);
        if (modifier.CanStackWithSelf || !modifierAlreadyActive) {
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
    }
}