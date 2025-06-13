using Cinemachine;
using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : StaticInstance<BossManager> {

    public static event Action OnStartBossFight;
    public static event Action OnBossKilled;
    public static event Action<GameObject> OnBossKilled_Boss;

    [SerializeField] private MMF_Player enterBossRoomPlayer;
    [SerializeField] private CinemachineVirtualCamera staticCamera;

    private GameObject boss;
    private EnemyHealth bossHealth;

    private PlayerHealth playerHealth;

    private bool FightingDealer => GameSceneManager.Instance.Level == 3;

    protected override void Awake() {
        base.Awake();
        playerHealth = PlayerMovement.Instance.GetComponent<PlayerHealth>();
    }

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TryStartBossFight;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryStartBossFight;
    }

    private void TryStartBossFight(Room room) {
        bool isBossRoom = room.TryGetComponent(out BossRoom bossRoom);

        if (isBossRoom && !room.IsRoomCleared) {
            StartBossFight(bossRoom);
        }
    }

    private void StartBossFight(BossRoom bossRoom) {

        GameStateManager.Instance.SetGameState(GameState.CutScene);

        ScriptableBoss scriptableBoss = SpawnBoss(bossRoom.GetBossSpawnPoint().position);

        staticCamera.transform.position = new Vector3(bossRoom.GetBossSpawnPoint().position.x, bossRoom.GetBossSpawnPoint().position.y, -10f);

        enterBossRoomPlayer.GetFeedbackOfType<MMF_HoldingPause>().AutoResume = !FightingDealer;
        enterBossRoomPlayer.Initialization();
        enterBossRoomPlayer.PlayFeedbacks();

        playerHealth.DeathEventTrigger.AddListener(OnPlayerDefeated);

        OnStartBossFight?.Invoke();
    }

    public void ResumeEnterBossPlayer() {
        enterBossRoomPlayer.ResumeFeedbacks();
    }

    private ScriptableBoss SpawnBoss(Vector2 spawnPoint) {
        List<ScriptableBoss> possibleBosses = ResourceSystem.Instance.GetBosses(GameSceneManager.Instance.CurrentEnvironment);
        ScriptableBoss chosenBoss = possibleBosses.RandomItem();

        if (!FightingDealer) {
            MonoBehaviour bossBehavior = chosenBoss.Prefab.GetComponent<IBoss>() as MonoBehaviour;
            bossBehavior.enabled = false;
        }

        boss = chosenBoss.Prefab.Spawn(spawnPoint, Containers.Instance.Enemies);

        bossHealth = boss.GetComponent<EnemyHealth>();

        //... setup the boss health bar
        BossHealthUI.Instance.Setup(chosenBoss.LocName, bossHealth);

        bossHealth.GetComponent<MonoBehaviourEventInvoker>().OnDisabled += OnBossDefeated;

        return chosenBoss;
    }

    // played by feedback
    public void OnEnterRoomPlayerCompleted() {
        GameStateManager.Instance.SetGameState(GameState.Game);

        if (!FightingDealer) {
            MonoBehaviour bossBehavior = boss.GetComponent<IBoss>() as MonoBehaviour;
            bossBehavior.enabled = true;
        }
    }

    private void OnBossDefeated(GameObject bossObject) {

        if (Helpers.GameStopping()) {
            return;
        }

        // hide healthbar
        FeedbackPlayerOld.PlayInReverse("BossHealthPopup");

        bossHealth.GetComponent<MonoBehaviourEventInvoker>().OnDisabled -= OnBossDefeated;
        playerHealth.DeathEventTrigger.RemoveListener(OnPlayerDefeated);

        EnemyHealth[] enemyHealths = Containers.Instance.Enemies.GetComponentsInChildren<EnemyHealth>();
        foreach (EnemyHealth health in enemyHealths) {
            health.Die();
        }

        OnBossKilled?.Invoke();
        OnBossKilled_Boss?.Invoke(bossObject);
    }

    private void OnPlayerDefeated() {
        bossHealth.GetComponent<MonoBehaviourEventInvoker>().OnDisabled -= OnBossDefeated;
        playerHealth.DeathEventTrigger.RemoveListener(OnPlayerDefeated);
    }
}
