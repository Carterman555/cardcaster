using Cinemachine;
using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    public static event Action OnStartBossFight;
    public static event Action OnBossKilled;

    [SerializeField] private MMF_Player enterBossRoomPlayer;
    [SerializeField] private CinemachineVirtualCamera staticCamera;
    [SerializeField] private BossHealthUI bossHealthUI;

    private MonoBehaviour boss;
    private EnemyHealth bossHealth;

    private PlayerHealth playerHealth;

    private Room room;

    private void Awake() {
        playerHealth = PlayerMovement.Instance.GetComponent<PlayerHealth>();
    }

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TryStartBossFight;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryStartBossFight;
    }

    private void TryStartBossFight(Room room) {
        this.room = room;

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

        playerHealth.DeathEventTrigger.AddListener(OnPlayerDefeated);

        OnStartBossFight?.Invoke();
    }

    private void SpawnBoss(Vector2 spawnPoint) {
        int currentLevel = GameSceneManager.Instance.GetLevel();
        List<ScriptableBoss> possibleBosses = ResourceSystem.Instance.GetBosses(currentLevel);
        ScriptableBoss chosenBoss = possibleBosses.RandomItem();

        GameObject bossObject = chosenBoss.Prefab.Spawn(spawnPoint, Containers.Instance.Enemies);
        boss = bossObject.GetComponent<IBoss>() as MonoBehaviour;
        bossHealth = boss.GetComponent<EnemyHealth>();

        //... setup the boss health bar
        bossHealthUI.Setup(chosenBoss.LocName, bossHealth);

        bossHealth.DeathEventTrigger.AddListener(OnBossDefeated);
    }

    public void OnEnterRoomPlayerCompleted() {
        GameStateManager.Instance.SetGameState(GameState.Game);

        //... enable boss
        boss.enabled = true;
    }

    private void OnBossDefeated() {

        boss.enabled = false;

        // hide healthbar
        FeedbackPlayerOld.PlayInReverse("BossHealthPopup");

        bossHealth.DeathEventTrigger.RemoveListener(OnBossDefeated);
        playerHealth.DeathEventTrigger.RemoveListener(OnPlayerDefeated);

        EnemyHealth[] enemyHealths = Containers.Instance.Enemies.GetComponentsInChildren<EnemyHealth>();
        foreach (EnemyHealth health in enemyHealths) {
            health.Die();
        }

        OnBossKilled?.Invoke();
    }

    private void OnPlayerDefeated() {
        bossHealth.DeathEventTrigger.RemoveListener(OnBossDefeated);
        playerHealth.DeathEventTrigger.RemoveListener(OnPlayerDefeated);
    }
}
