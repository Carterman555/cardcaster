using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : MonoBehaviour {

    [SerializeField] private ShopCard shopItemPrefab;
    [SerializeField] private Transform[] itemSpawnPoints;

    private void OnEnable() {
        SpawnCards();
    }

    private void SpawnCards() {
        foreach (Transform spawnPoint in itemSpawnPoints) {
            ShopCard shopItem = shopItemPrefab.Spawn(spawnPoint.position, transform);

            int currentLevel = GameSceneManager.Instance.GetLevel();
            List<ScriptableCardBase> possibleCards = ResourceSystem.Instance.GetUnlockedCardsUpToLevel(currentLevel);
            ScriptableCardBase randomCard = possibleCards.RandomItem();

            shopItem.SetCard(randomCard);
        }
    }
}
