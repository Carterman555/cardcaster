using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : MonoBehaviour {

    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private Transform[] itemSpawnPoints;

    private void OnEnable() {
        SpawnItems();
    }

    private void SpawnItems() {
        foreach (Transform spawnPoint in itemSpawnPoints) {
            ShopItem shopItem = shopItemPrefab.Spawn(spawnPoint.position, transform);

            Level currentLevel = Level.Level1; // change with levelmanager is created
            List<ScriptableCardBase> possibleCards = ResourceSystem.Instance.GetPossibleCards(currentLevel);
            ScriptableCardBase randomCard = possibleCards.RandomItem();

            shopItem.SetCard(randomCard);
        }
    }
}
