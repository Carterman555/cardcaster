using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : StaticInstance<AbilityManager> {

    private void Start() {
        EnsureAbilityIncompatibilitiesMatch();
    }

    #region Abilities

    private void EnsureAbilityIncompatibilitiesMatch() {

        ScriptableAbilityCardBase[] allAbilityCards = ResourceSystem.Instance.AllCards
            .Where(c => c is ScriptableAbilityCardBase abilityCard)
            .Select(c => (ScriptableAbilityCardBase)c).ToArray();

        List<ScriptableAbilityCardBase> remainingAbilityCards = allAbilityCards.ToList();
        remainingAbilityCards.Reverse();

        foreach (var abilityCard1 in allAbilityCards) {
            foreach (var abilityCard2 in remainingAbilityCards) {
                bool card1Contains2 = abilityCard1.IncompatibleAbilities != null && abilityCard1.IncompatibleAbilities.Contains(abilityCard2.CardType);
                bool card2Contains1 = abilityCard2.IncompatibleAbilities != null && abilityCard2.IncompatibleAbilities.Contains(abilityCard1.CardType);

                if (card1Contains2 && !card2Contains1) {
                    Debug.LogWarning($"Ability Incompatibilities Mismatch: {abilityCard1.CardType} contains {abilityCard2.CardType}, " +
                        $"but {abilityCard2.CardType} does not contain {abilityCard1.CardType}");
                }

                if (card2Contains1 && !card1Contains2) {
                    Debug.LogWarning($"Ability Incompatibilities Mismatch: {abilityCard2.CardType} contains {abilityCard1.CardType}, " +
                        $"but {abilityCard1.CardType} does not contain {abilityCard2.CardType}");
                }
            }

            remainingAbilityCards.Remove(abilityCard1);
        }
    }

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
        alreadyActiveAbility = activeAbilities.FirstOrDefault(a => a.CardType == ability.CardType);
        return alreadyActiveAbility != null;
    }

    public bool IsAbilityActive(ScriptableAbilityCardBase ability) {
        return IsAbilityActive(ability.CardType);
    }

    public bool IsAbilityActive(CardType cardType) {
        return activeAbilities.Any(a => a.CardType == cardType);
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
        if (modifier.StackType == StackType.Stackable || !IsModifierActive(modifier)) {
            activeModifiers.Add(modifier);
        }
    }

    public void ApplyModifiers(ScriptableAbilityCardBase card) {
        foreach (var modifier in activeModifiers) {
            if (card.IsCompatibleWithModifier(modifier)) {
                modifier.ApplyToAbility(card);
            }
        }
        activeModifiers.Clear();

        OnApplyModifiers?.Invoke();
    }

    #endregion
}