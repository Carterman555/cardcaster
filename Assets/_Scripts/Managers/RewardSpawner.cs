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
        BossManager.OnBossKilled += SpawnBossReward;
    }
    private void OnDisable() {
        CheckEnemiesCleared.OnEnemiesCleared -= TrySpawnReward;
        BossManager.OnBossKilled -= SpawnBossReward;
    }

    private void TrySpawnReward() {
        if (Random.value < rewardOnClearChance) {
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
            chest.GetComponent<CreateMapIcon>().ShowMapIcon();
        }
        else {
            // spawn campfire
            Campfire campfire = campfirePrefab.Spawn(position, Containers.Instance.Drops);
            campfire.GetComponent<CreateMapIcon>().ShowMapIcon();
        }
    }

    [Header("Boss Loot")]
    [SerializeField] private bool bossUnlocksCardIfPossible = true;
    [SerializeField] private CardDrop cardDropPrefab;

    [Command]
    private void SpawnBossReward() {
        bool inBossRoom = Room.GetCurrentRoom().TryGetComponent(out BossRoom bossRoom);
        if (!inBossRoom) {
            Debug.LogError("Tried spawning boss reward while not in boss room!");
            return;
        }

        int currentLevel = GameSceneManager.Instance.GetLevel();
        List<ScriptableCardBase> possibleCardsToSpawn = ResourceSystem.Instance.GetAllCardsWithLevel(currentLevel);

        if (bossUnlocksCardIfPossible) {

            List<ScriptableCardBase> unlockedCards = ResourceSystem.Instance.GetUnlockedCardsWithLevel(currentLevel);
            bool unlockedAllCardsAtLevel = possibleCardsToSpawn.Count == unlockedCards.Count;

            if (!unlockedAllCardsAtLevel) {
                possibleCardsToSpawn = possibleCardsToSpawn.Where(c => !unlockedCards.Contains(c)).ToList();
            }
        }

        CardDrop newCardDrop = cardDropPrefab.Spawn(bossRoom.GetBossSpawnPoint().position, Containers.Instance.Drops);

        ScriptableCardBase scriptableCard = possibleCardsToSpawn.RandomItem();
        newCardDrop.SetCard(scriptableCard);
    }
}
