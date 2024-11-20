using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : MonoBehaviour {

    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private Transform[] itemSpawnPoints;

    private void OnEnable() {
        SpawnCards();
    }

    private void SpawnCards() {
        foreach (Transform spawnPoint in itemSpawnPoints) {
            ShopItem shopItem = shopItemPrefab.Spawn(spawnPoint.position, transform);

            int currentLevel = LevelManager.Instance.GetLevel();
            List<ScriptableCardBase> possibleCards = ResourceSystem.Instance.GetUnlockedCardsUpToLevel(currentLevel);
            ScriptableCardBase randomCard = possibleCards.RandomItem();

            shopItem.SetCard(randomCard);
        }
    }
}
