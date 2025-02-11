using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShopCardSpawn : MonoBehaviour {

    [SerializeField] private ShopCard shopItemPrefab;

    private void OnEnable() {
        SpawnCard();
    }

    private void SpawnCard() {
        ShopCard shopItem = shopItemPrefab.Spawn(transform.position, transform);

        int currentLevel = GameSceneManager.Instance.GetLevel();
        List<ScriptableCardBase> possibleCards = ResourceSystem.Instance.GetUnlockedCardsUpToLevel(currentLevel);
        ScriptableCardBase randomCard = possibleCards.RandomItem();

        shopItem.SetCard(randomCard);
    }
}
