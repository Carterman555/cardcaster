using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManagerOld : StaticInstance<AbilityManagerOld> {

    private List<CardType> activeCards = new List<CardType>();

    public void AddActiveCard(CardType cardType) {
        activeCards.Add(cardType);
    }

    public void RemoveActiveCard(CardType cardType) {

        if (!activeCards.Contains(cardType)){
            Debug.LogWarning("Trying To Remove Active Card That Is Not In List!");
        }

        activeCards.Remove(cardType);
    }


    public bool IsCardActive(CardType cardType) {
        return activeCards.Contains(cardType);
    }
}
