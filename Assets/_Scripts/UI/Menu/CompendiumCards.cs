using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompendiumCards : MonoBehaviour {

    [SerializeField] private Transform cardContainer;
    [SerializeField] private CompendiumCard compendiumCardPrefab;

    private List<CompendiumCard> compendiumCards;

    [SerializeField] private List<CardType> cardTypes;

    private void OnEnable() {
        compendiumCards = new();
        foreach (CardType cardType in cardTypes) {
            CompendiumCard compendiumCard = compendiumCardPrefab.Spawn(cardContainer);

            bool locked = !ResourceSystem.Instance.UnlockedCards.Contains(cardType);
            compendiumCard.Setup(cardType, locked);
            compendiumCards.Add(compendiumCard);
        }
    }

    private void OnDisable() {
        foreach (CompendiumCard compendiumCard in compendiumCards) {
            compendiumCard.gameObject.ReturnToPool();
        }
        compendiumCards.Clear();
    }

    [ContextMenu("Reset Card Types")]
    private void ResetCardTypes() {

        ScriptableCardBase[] abilityCards = ResourceSystem.Instance.AllCards.Where(c => c is ScriptableAbilityCardBase).OrderBy(c => c.name).ToArray();
        ScriptableCardBase[] modifierCards = ResourceSystem.Instance.AllCards.Where(c => c is ScriptableModifierCardBase).OrderBy(c => c.name).ToArray();
        ScriptableCardBase[] persistentCards = ResourceSystem.Instance.AllCards.Where(c => c is ScriptablePersistentCard).OrderBy(c => c.name).ToArray();

        cardTypes = new();

        cardTypes.AddRange(abilityCards.Select(c => c.CardType));
        cardTypes.AddRange(modifierCards.Select(c => c.CardType));
        cardTypes.AddRange(persistentCards.Select(c => c.CardType));

    }
}
