using DG.Tweening;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardSpawner : MonoBehaviour {

    [SerializeField][Range(0f, 1f)] private float rewardOnClearChance;
    [SerializeField][Range(0f, 1f)] private float chestRewardChance;

    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Campfire campfirePrefab;

    private void OnEnable() {
        CheckEnemiesCleared.OnEnemiesCleared += TrySpawnReward;
    }
    private void OnDisable() {
        CheckEnemiesCleared.OnEnemiesCleared -= TrySpawnReward;
    }

    private void TrySpawnReward() {

        bool clearedBossRoom = Room.GetCurrentRoom().TryGetComponent(out BossRoom bossRoom);

        if (!clearedBossRoom) {
            if (Random.value < rewardOnClearChance) {
                SpawnReward();
            }
        }
        else {
            SpawnBossLoot(bossRoom.GetBossSpawnPoint().position);
        }
    }

    [ContextMenu("Rewards")]
    private void DebugSpawnReward() {
        for (int i = 0; i < 100; i++)
        {
            SpawnReward();
        }
    }

    [Command]
    private void SpawnReward() {

        float avoidPlayerRadius = 2f;
        float obstacleAvoidanceRadius = 3.5f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.transform.position, avoidPlayerRadius, obstacleAvoidanceRadius);

        if (Random.value < chestRewardChance) {
            // Instantiate the chest instead of using the spawning pool because the item that gets chosen gets unparented. And it's easiest to
            // just reset the chest by instantiating and destroying
            Chest chest = Instantiate(chestPrefab, position, Quaternion.identity, Containers.Instance.Drops);
        }
        else {
            // spawn campfire
            Campfire campfire = campfirePrefab.Spawn(position, Containers.Instance.Drops);
        }
    }

    [Header("Boss Loot")]
    [SerializeField] private bool bossUnlocksCardIfPossible = true;
    [SerializeField] private CardDrop cardDropPrefab;

    [Command]
    private void SpawnBossLoot(Vector2 spawnPoint) {

        int currentLevel = GameSceneManager.Instance.GetLevel();
        List<ScriptableCardBase> possibleCardsToSpawn = ResourceSystem.Instance.GetAllCardsWithLevel(currentLevel);

        if (bossUnlocksCardIfPossible) {

            List<ScriptableCardBase> unlockedCards = ResourceSystem.Instance.GetUnlockedCardsWithLevel(currentLevel);
            bool unlockedAllCardsAtLevel = possibleCardsToSpawn.Count == unlockedCards.Count;

            if (!unlockedAllCardsAtLevel) {
                possibleCardsToSpawn = possibleCardsToSpawn.Where(c => !unlockedCards.Contains(c)).ToList();
            }
        }

        CardDrop newCardDrop = cardDropPrefab.Spawn(spawnPoint, Containers.Instance.Drops);

        ScriptableCardBase scriptableCard = possibleCardsToSpawn.RandomItem();
        newCardDrop.SetCard(scriptableCard);
    }
}
