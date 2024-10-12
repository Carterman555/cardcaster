using DG.Tweening;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSpawner : MonoBehaviour {

    [SerializeField][Range(0f, 1f)] private float rewardOnClearChance;
    [SerializeField][Range(0f, 1f)] private float chestRewardChance;

    [SerializeField] private Chest chestPrefab;
    [SerializeField] private Campfire campfirePrefab;

    private void OnEnable() {
        CheckRoomCleared.OnEnemiesCleared += TrySpawnReward;
    }
    private void OnDisable() {
        CheckRoomCleared.OnEnemiesCleared -= TrySpawnReward;
    }

    private void TrySpawnReward() {
        if (Random.value < rewardOnClearChance) {
            SpawnReward();
        }
    }

    [Command]
    private void SpawnReward() {

        float avoidPlayerRadius = 2f;
        Vector2 position = new RoomPositionHelper().GetRandomSpawnPos(PlayerMovement.Instance.transform.position, avoidPlayerRadius);

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
}
