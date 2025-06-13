using System.Collections.Generic;
using UnityEngine;

public class ShopCardSpawn : MonoBehaviour {

    [SerializeField] private ShopCard shopItemPrefab;

    private void OnEnable() {
        SpawnCard();
    }

    private void SpawnCard() {
        ShopCard shopItem = shopItemPrefab.Spawn(transform.position, transform);

        List<CardType> possibleCards = ResourceSystem.Instance.GetUnlockedRewardCards();
        if (!RewardSpawner.CanGainOpenPalmsCard() && possibleCards.Contains(CardType.OpenPalms)) {
            possibleCards.Remove(CardType.OpenPalms);
        }

        CardType choosenCardType = ResourceSystem.Instance.GetRandomCardWeighted(possibleCards);
        ScriptableCardBase choosenCard = ResourceSystem.Instance.GetCardInstance(choosenCardType);

        shopItem.SetCard(choosenCard);
    }
}
