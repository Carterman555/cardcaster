using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardSpawner : MonoBehaviour {

    [Header("Chests and Campfires")]
    [SerializeField][Range(0f, 1f)] private float rewardOnClearChance;
    [SerializeField][Range(0f, 1f)] private float startingChestChance;
    private float chestChance;

    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Chest persistentChestPrefab;
    [SerializeField] private Campfire campfirePrefab;

    [Header("Shiny Goblins")]
    [SerializeField, Range(0f, 1f)] private float startingShinyGoblinChance;
    private float shinyGoblinChance;

    [SerializeField] private Enemy shinyGoblinPrefab;

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TrySpawnShinyGoblin;
        CheckEnemiesCleared.OnEnemiesCleared += TrySpawnReward;
        BossManager.OnBossKilled_Boss += OnBossKilled;
        
        chestChance = startingChestChance;
        shinyGoblinChance = startingShinyGoblinChance;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TrySpawnShinyGoblin;
        CheckEnemiesCleared.OnEnemiesCleared -= TrySpawnReward;
        BossManager.OnBossKilled_Boss -= OnBossKilled;
    }

    private void TrySpawnShinyGoblin(Room room) {

        if (room.IsRoomCleared) {
            return;
        }

        RoomType[] validRoomTypes = new RoomType[] {
            RoomType.Normal,
            RoomType.Hub
        };

        if (validRoomTypes.Contains(room.ScriptableRoom.RoomType)) {

            float balanceIncrement = 0.5f;
            if (shinyGoblinChance > Random.value) {

                Vector2 pos = new RoomPositionHelper()
                    .SetAvoidArea(center: PlayerMovement.Instance.CenterPos, radius: 2f)
                    .SetObstacleAvoidance(1f)
                    .SetWallAvoidance(1f)
                    .SetEntranceAvoidance(3f)
                    .GetRandomPositionInCollider(inCameraView: true);

                EnemySpawner.Instance.SpawnEnemy(shinyGoblinPrefab, pos, createSpawnEffect: false);
                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.SpawnShinyGoblin);
                shinyGoblinChance -= balanceIncrement;
            }
            else {
                shinyGoblinChance += balanceIncrement;
            }
        }
    }

    private void TrySpawnReward() {
        bool inBossRoom = Room.GetCurrentRoom().TryGetComponent(out BossRoom bossRoom);
        if (!inBossRoom && rewardOnClearChance > Random.value) {
            SpawnReward();
        }
    }

    [Command]
    private void SpawnReward() {

        float balanceIncrement = 0.1f;

        if (Random.value < chestChance) {
            SpawnChest();
            chestChance -= balanceIncrement;
        }
        else {
            SpawnCampfire();
            chestChance += balanceIncrement;
        }
    }

    [Command]
    private void SpawnChest() {
        float avoidPlayerRadius = 2f;
        float obstacleAvoidanceRadius = 3.5f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.CenterPos, avoidPlayerRadius, obstacleAvoidanceRadius);

        // Instantiate the chest instead of using the spawning pool because the item that gets chosen gets unparented. And it's easiest to
        // just reset the chest by instantiating and destroying
        Chest chest = Instantiate(chestPrefab, position, Quaternion.identity, Containers.Instance.Drops);
        chest.GetComponent<CreateMapIcon>().ShowMapIcon();
    }

    [Command]
    private void SpawnCampfire() {
        float avoidPlayerRadius = 2f;
        float obstacleAvoidanceRadius = 3.5f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.CenterPos, avoidPlayerRadius, obstacleAvoidanceRadius);
        Campfire campfire = campfirePrefab.Spawn(position, Containers.Instance.Drops);
        campfire.GetComponent<CreateMapIcon>().ShowMapIcon();
    }

    [Header("Boss Loot")]
    [SerializeField] private CardUnlockDrop cardUnlockDropPrefab;
    [SerializeField] private Vector2 persistentChestOffset;

    private void OnBossKilled(GameObject boss) {
        StartCoroutine(BossKilledCor());
    }

    private IEnumerator BossKilledCor() {

        float spawnBossLootDelay = 1.5f;
        yield return new WaitForSeconds(spawnBossLootDelay);

        bool inBossRoom = Room.GetCurrentRoom().TryGetComponent(out BossRoom bossRoom);
        if (!inBossRoom) {
            Debug.LogError("Tried spawning boss reward while not in boss room!");
            yield break;
        }

        SpawnCardUnlockDrop(bossRoom.GetBossSpawnPoint().position);

        Vector2 chestPos = (Vector2)bossRoom.GetBossSpawnPoint().position + persistentChestOffset;
        Chest chest = Instantiate(persistentChestPrefab, chestPos, Quaternion.identity, Containers.Instance.Drops);
        chest.GetComponent<CreateMapIcon>().ShowMapIcon();
    }

    [Command]
    private void SpawnCardUnlockDrop(Vector2 position) {
        EnvironmentType currentEnvironment = GameSceneManager.Instance.CurrentEnvironment;
        List<CardType> possibleCardsToSpawn = ResourceSystem.Instance.GetLockedRewardCards(currentEnvironment);

        bool unlockedAllCardsAtLevel = possibleCardsToSpawn.Count == 0;
        if (unlockedAllCardsAtLevel) {
            print("unlockedAllCardsAtLevel");
            return;
        }

        CardType choosenCardType = ResourceSystem.Instance.GetRandomCardWeighted(possibleCardsToSpawn);
        CardUnlockDrop newCardUnlock = cardUnlockDropPrefab.Spawn(position, Containers.Instance.Drops);
        newCardUnlock.SetCard(ResourceSystem.Instance.GetCardInstance(choosenCardType));
    }
}
