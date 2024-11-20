using Cinemachine;
using MoreMountains.Feedbacks;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    [SerializeField] private MMF_Player enterBossRoomPlayer;
    [SerializeField] private CinemachineVirtualCamera staticCamera;
    [SerializeField] private BossHealthUI bossHealthUI;

    private MonoBehaviour boss;
    private Health bossHealth;

    private Health playerHealth;

    private void Awake() {
        playerHealth = PlayerMovement.Instance.GetComponent<Health>();
    }

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TryStartBossFight;
    }
    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryStartBossFight;
    }

    private void TryStartBossFight(Room room) {

        bool isBossRoom = room.TryGetComponent(out BossRoom bossRoom);

        if (isBossRoom && !room.IsRoomCleared()) {
            StartBossFight(bossRoom);
        }
    }

    private void StartBossFight(BossRoom bossRoom) {

        GameStateManager.Instance.SetGameState(GameState.CutScene);

        SpawnBoss(bossRoom.GetBossSpawnPoint().position);

        staticCamera.transform.position = new Vector3(bossRoom.GetBossSpawnPoint().position.x, bossRoom.GetBossSpawnPoint().position.y, -10f);

        enterBossRoomPlayer.PlayFeedbacks();

        playerHealth.OnDeath += OnPlayerDefeated;
    }

    private void SpawnBoss(Vector2 spawnPoint) {
        int currentLevel = LevelManager.Instance.GetLevel();
        List<ScriptableBoss> possibleBosses = ResourceSystem.Instance.GetBosses(currentLevel);
        ScriptableBoss chosenBoss = possibleBosses.RandomItem();

        GameObject bossObject = chosenBoss.Prefab.Spawn(spawnPoint, Containers.Instance.Enemies);
        boss = bossObject.GetComponent<IBoss>() as MonoBehaviour;
        bossHealth = boss.GetComponent<Health>();

        //... setup the boss health bar
        bossHealthUI.Setup(chosenBoss.Name, bossHealth);

        bossHealth.OnDeath += OnBossDefeated;
    }

    public void OnEnterRoomPlayerCompleted() {
        GameStateManager.Instance.SetGameState(GameState.Game);

        //... enable boss
        boss.enabled = true;
    }

    private void OnBossDefeated() {

        boss.enabled = false;

        // hide healthbar
        FeedbackPlayer.PlayInReverse("BossHealthPopup");

        bossHealth.OnDeath -= OnBossDefeated;
        playerHealth.OnDeath -= OnPlayerDefeated;
    }

    private void OnPlayerDefeated() {
        bossHealth.OnDeath -= OnBossDefeated;
        playerHealth.OnDeath -= OnPlayerDefeated;
    }
}
