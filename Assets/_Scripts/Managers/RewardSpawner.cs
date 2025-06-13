using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardSpawner : MonoBehaviour {

    [SerializeField][Range(0f, 1f)] private float rewardOnClearChance;
    [SerializeField][Range(0f, 1f)] private float startingChestChance;
    private float chestChance;

    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Chest persistentChestPrefab;
    [SerializeField] private Campfire campfirePrefab;

    private static bool usedOpenPalmsCard;

    private void OnEnable() {
        CheckEnemiesCleared.OnEnemiesCleared += TrySpawnReward;
        BossManager.OnBossKilled_Boss += OnBossKilled;

        HandCard.OnAnyCardUsed_Card += OnUseCard;
        GameSceneManager.OnStartGameLoadingCompleted += ResetUsedOpenPalmsCard;

        chestChance = startingChestChance;
    }

    private void OnDisable() {
        CheckEnemiesCleared.OnEnemiesCleared -= TrySpawnReward;
        BossManager.OnBossKilled_Boss -= OnBossKilled;

        HandCard.OnAnyCardUsed_Card -= OnUseCard;
        GameSceneManager.OnStartGameLoadingCompleted -= ResetUsedOpenPalmsCard;
    }

    private void TrySpawnReward() {
        bool inBossRoom = Room.GetCurrentRoom().TryGetComponent(out BossRoom bossRoom);
        if (!inBossRoom && Random.value < rewardOnClearChance) {
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

    

    private void OnUseCard(ScriptableCardBase card) {
        if (card is ScriptableOpenPalmsCard) {
            usedOpenPalmsCard = true;
        }
    }

    private void ResetUsedOpenPalmsCard() {
        usedOpenPalmsCard = false;
    }

    public static bool CanGainOpenPalmsCard() {
        bool hasOpenPalmsCard = DeckManager.Instance.GetAllCards().Any(c => c is ScriptableOpenPalmsCard);
        return !usedOpenPalmsCard && !hasOpenPalmsCard;
    }
}
