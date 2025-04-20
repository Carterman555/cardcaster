using System.Collections.Generic;
using UnityEngine;

public class ShopCardSpawn : MonoBehaviour {

    [SerializeField] private ShopCard shopItemPrefab;

    private void OnEnable() {
        SpawnCard();
    }

    private void SpawnCard() {
        ShopCard shopItem = shopItemPrefab.Spawn(transform.position, transform);

        List<CardType> possibleCards = ResourceSystem.Instance.GetUnlockedCards();
        if (!RewardSpawner.CanGainWisdomCard() && possibleCards.Contains(CardType.WisdomsHold)) {
            possibleCards.Remove(CardType.WisdomsHold);
        }

        CardType choosenCardType = ResourceSystem.Instance.GetRandomCardWeighted(possibleCards);
        ScriptableCardBase choosenCard = ResourceSystem.Instance.GetCardInstance(choosenCardType);

        shopItem.SetCard(choosenCard);
    }
}
