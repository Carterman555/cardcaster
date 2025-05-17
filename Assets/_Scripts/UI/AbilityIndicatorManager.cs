using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityIndicatorManager : StaticInstance<AbilityIndicatorManager> {

    [SerializeField] private AbilityIndicator abilityIndicatorPrefab;

    private List<AbilityIndicator> abilityIndicators = new();

    public void AddIndicator(ScriptableAbilityCardBase abilityCard) {
        AbilityIndicator abilityIndicator = abilityIndicatorPrefab.Spawn(transform);
        abilityIndicator.Setup(abilityCard);

        abilityIndicators.Add(abilityIndicator);
    }

    public void RemoveIndicatorFromList(AbilityIndicator abilityIndicator) {
        if (!abilityIndicators.Contains(abilityIndicator)) {
            Debug.LogError("Trying to remove ability indicator that is not in the list!");
            return;
        }

        abilityIndicators.Remove(abilityIndicator);
    }

    public void ResetDurationOfIndicator(ScriptableAbilityCardBase abilityCard) {
        AbilityIndicator[] matchingAbilityIndicators = abilityIndicators.Where(i => i.CardType == abilityCard.CardType).ToArray();

        if (matchingAbilityIndicators.Length == 0) {
            Debug.LogError("Trying to reset duration of ability indicator that is not in the list!");
            return;
        }

        if (matchingAbilityIndicators.Length > 1) {
            Debug.LogError("Trying to reset duration of ability indicator when there are multiple active!");
            return;
        }

        matchingAbilityIndicators[0].ResetDuration(abilityCard);
    }
}
