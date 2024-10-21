using Cinemachine;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    [SerializeField] private MMF_Player enterBossRoomPlayer;

    [SerializeField] private CinemachineVirtualCamera staticCamera; 

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TryStartBossFight;
    }
    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryStartBossFight;
    }

    private void TryStartBossFight(Room room) {
        if (room.TryGetComponent(out BossRoom bossRoom)) {
            StartBossFight(bossRoom);
        }
    }

    private void StartBossFight(BossRoom bossRoom) {

        GameStateManager.Instance.SetGameState(GameState.CutScene);

        SpawnBoss(bossRoom.GetBossSpawnPoint().position);

        staticCamera.transform.position = new Vector3(bossRoom.GetBossSpawnPoint().position.x, bossRoom.GetBossSpawnPoint().position.y, -10f);

        enterBossRoomPlayer.PlayFeedbacks();
    }

    private void SpawnBoss(Vector2 spawnPoint) {
        Level currentLevel = Level.Level1;
        ScriptableBoss[] possibleBosses = ResourceSystem.Instance.GetBosses(currentLevel);
        ScriptableBoss chosenBoss = possibleBosses.RandomItem();

        chosenBoss.Prefab.Spawn(spawnPoint, Containers.Instance.Enemies);
    }

    public void OnEnterRoomPlayerCompleted() {
        GameStateManager.Instance.SetGameState(GameState.Game);

        // enable boss
    }
}
