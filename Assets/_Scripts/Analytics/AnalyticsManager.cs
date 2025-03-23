using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using System;
using Unity.Services.Core;
using static Cinemachine.DocumentationSortingAttribute;

public class AnalyticsManager : MonoBehaviour {

    async void Awake() {

#if UNITY_EDITOR
        if (!playInEditor) {
            return;
        }
#endif

        try {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e) {
            Debug.LogException(e);
        }

        AnalyticsService.Instance.StartDataCollection();
    }

    #region Record Events

    private void OnEnable() {

#if UNITY_EDITOR
        if (!playInEditor) {
            return;
        }
#endif

        GameSceneManager.OnStartGame += SetStartTimes;

        playerHealth.DeathEventTrigger.AddListener(RecordDeathEvent);

        GameSceneManager.OnLevelComplete += RecordLevelCompleteEvent;

        Room.OnAnyRoomEnter_Room += OnRoomEnter;
        CheckEnemiesCleared.OnEnemiesCleared += RecordRoomEndEvent;
        playerHealth.DeathEventTrigger.AddListener(RecordRoomEndEvent);

        GameSceneManager.OnWinGame += RecordCompleteGameEvent;
    }

    private void OnDisable() {

#if UNITY_EDITOR
        if (!playInEditor) {
            return;
        }
#endif

        GameSceneManager.OnStartGame -= SetStartTimes;

        playerHealth.DeathEventTrigger.RemoveListener(RecordDeathEvent);

        GameSceneManager.OnLevelComplete -= RecordLevelCompleteEvent;

        Room.OnAnyRoomEnter_Room -= OnRoomEnter;
        CheckEnemiesCleared.OnEnemiesCleared -= RecordRoomEndEvent;
        playerHealth.DeathEventTrigger.RemoveListener(RecordRoomEndEvent);

        GameSceneManager.OnWinGame -= RecordCompleteGameEvent;
    }

    [SerializeField] private bool playInEditor = false;
    [SerializeField] private bool debug = true;

    private float GameTime => Time.time - startRunTime;
    private static float startRunTime; // static so doesn't reset on scene load

    private void SetStartTimes() {
        startRunTime = Time.time;
        startLevelTime = Time.time;
    }


    [SerializeField] private PlayerHealth playerHealth;

    private void RecordDeathEvent() {
        PlayerDeathEvent playerDeathEvent = new() {
            RunTime = GameTime,
            Room = Room.GetCurrentRoom().name,
            Level = GameSceneManager.Instance.GetLevel()
        };

        AnalyticsService.Instance.RecordEvent(playerDeathEvent);
        if (debug) print($"Record death: {GameTime}, {Room.GetCurrentRoom().name}, {GameSceneManager.Instance.GetLevel()}");
    }


    private static float startLevelTime; // static so doesn't reset on scene load

    private void RecordLevelCompleteEvent(int level) {

        LevelCompleteEvent levelCompleteEvent = new() {
            TimeInLevel = Time.time - startLevelTime,
            Level = level
        };

        AnalyticsService.Instance.RecordEvent(levelCompleteEvent);
        if (debug) print($"Record level complete: {Time.time - startLevelTime}, {level}");

        startLevelTime = Time.time;
    }


    private float timeAtRoomEnter;
    private float healthAtRoomEnter;

    private void OnRoomEnter(Room room) {
        timeAtRoomEnter = Time.time;
        healthAtRoomEnter = playerHealth.CurrentHealth;
    }

    private void RecordRoomEndEvent() {
        RoomEndEvent roomEndEvent = new() {
            Room = Room.GetCurrentRoom().name,
            TimeToEnd = Time.time - timeAtRoomEnter,
            HealthLost = healthAtRoomEnter - playerHealth.CurrentHealth,
            Survived = !playerHealth.Dead
        };

        AnalyticsService.Instance.RecordEvent(roomEndEvent);
        if (debug) print($"Record room end: Room = {Room.GetCurrentRoom().name} Time = {Time.time - timeAtRoomEnter}, Health lost = {healthAtRoomEnter - playerHealth.CurrentHealth}, Survived = {!playerHealth.Dead}");
    }


    private void RecordCompleteGameEvent() {
        CompleteGameEvent completeGameEvent = new() {
            TimeToComplete = GameTime
        };

        AnalyticsService.Instance.RecordEvent(completeGameEvent);
        if (debug) print($"Record game complete: {GameTime}");
    }

    #endregion
}
