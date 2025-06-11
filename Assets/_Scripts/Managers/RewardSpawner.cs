using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardSpawner : MonoBehaviour {

    [SerializeField][Range(0f, 1f)] private float rewardOnClearChance;
    [SerializeField][Range(0f, 1f)] private float startingChestChance;
    private float chestChance;

    [SerializeField] private Chest chestPrefab;
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
        if (Random.value < rewardOnClearChance) {
            SpawnReward();
        }
    }

    [Command]
    private void SpawnReward() {

        float avoidPlayerRadius = 2f;
        float obstacleAvoidanceRadius = 3.5f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.CenterPos, avoidPlayerRadius, obstacleAvoidanceRadius);

        float balanceIncrement = 0.1f;

        if (Random.value < chestChance) {
            // Instantiate the chest instead of using the spawning pool because the item that gets chosen gets unparented. And it's easiest to
            // just reset the chest by instantiating and destroying
            Chest chest = Instantiate(chestPrefab, position, Quaternion.identity, Containers.Instance.Drops);
            chest.GetComponent<CreateMapIcon>().ShowMapIcon();

            chestChance -= balanceIncrement;
        }
        else {
            // spawn campfire
            Campfire campfire = campfirePrefab.Spawn(position, Containers.Instance.Drops);
            campfire.GetComponent<CreateMapIcon>().ShowMapIcon();

            chestChance += balanceIncrement;
        }
    }

    [Command]
    private void SpawnChestDebug() {
        float avoidPlayerRadius = 2f;
        float obstacleAvoidanceRadius = 3.5f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.CenterPos, avoidPlayerRadius, obstacleAvoidanceRadius);
        Chest chest = Instantiate(chestPrefab, position, Quaternion.identity, Containers.Instance.Drops);
        chest.GetComponent<CreateMapIcon>().ShowMapIcon();
    }

    [Command]
    private void SpawnCampfireDebug() {
        float avoidPlayerRadius = 2f;
        float obstacleAvoidanceRadius = 3.5f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.CenterPos, avoidPlayerRadius, obstacleAvoidanceRadius);
        Campfire campfire = Instantiate(campfirePrefab, position, Quaternion.identity, Containers.Instance.Drops);
        campfire.GetComponent<CreateMapIcon>().ShowMapIcon();
    }

    [Header("Boss Loot")]
    [SerializeField] private bool bossUnlocksCardIfPossible = true;
    [SerializeField] private CardDrop cardDropPrefab;

    private void OnBossKilled(GameObject boss) {
        SpawnBossReward();
    }

    [Command]
    private void SpawnBossReward() {
        bool inBossRoom = Room.GetCurrentRoom().TryGetComponent(out BossRoom bossRoom);
        if (!inBossRoom) {
            Debug.LogError("Tried spawning boss reward while not in boss room!");
            return;
        }

        int currentLevel = GameSceneManager.Instance.Level;
        List<CardType> possibleCardsToSpawn = ResourceSystem.Instance.GetUnlockedCards();

        if (bossUnlocksCardIfPossible) {
            List<CardType> unlockedCards = ResourceSystem.Instance.GetUnlockedCardsWithLevel(currentLevel);
            List<CardType> allCardsAtLevel = ResourceSystem.Instance.GetAllCardsWithLevel(currentLevel);
            bool unlockedAllCardsAtLevel = allCardsAtLevel.Count == unlockedCards.Count;

            if (!unlockedAllCardsAtLevel) {
                possibleCardsToSpawn = allCardsAtLevel.Where(c => !unlockedCards.Contains(c)).ToList();
            }
        }

        if (!CanGainOpenPalmsCard() && possibleCardsToSpawn.Contains(CardType.OpenPalms)) {
            possibleCardsToSpawn.Remove(CardType.OpenPalms);
        }

        CardType choosenCardType = ResourceSystem.Instance.GetRandomCardWeighted(possibleCardsToSpawn);
        StartCoroutine(SpawnBossCardCor(bossRoom.GetBossSpawnPoint().position, ResourceSystem.Instance.GetCardInstance(choosenCardType)));
    }

    private IEnumerator SpawnBossCardCor(Vector2 position, ScriptableCardBase scriptableCard) {

        float spawnBossLootDelay = 1.5f;
        yield return new WaitForSeconds(spawnBossLootDelay);

        CardDrop newCardDrop = cardDropPrefab.Spawn(position, Containers.Instance.Drops);
        newCardDrop.SetCard(scriptableCard);
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
