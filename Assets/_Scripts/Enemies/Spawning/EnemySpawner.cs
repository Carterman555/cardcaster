using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : StaticInstance<EnemySpawner> {

    private ScriptableEnemyComposition currentEnemyComposition;

    private int currentWaveIndex;

    public bool SpawnedAllWaves() {
        int totalWaves = currentEnemyComposition.EnemyWaves.Count();
        return currentWaveIndex >= totalWaves;
    }

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TrySpawnEnemies;
    }
    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TrySpawnEnemies;
    }

    private void TrySpawnEnemies(Room room) {

        // don't spawn in boss room
        if (room.TryGetComponent(out BossRoom bossRoom)) {
            return;
        }

        // don't spawn if room is already cleared
        if (room.IsRoomCleared()) {
            return;
        }

        currentEnemyComposition = room.GetScriptableRoom().ScriptableEnemyComposition;
        currentWaveIndex = 0;

        SpawnCurrentWave();
    }

    public void SpawnCurrentWave() {

        if (SpawnedAllWaves()) {
            Debug.LogWarning("Trying to spawn current wave, but all waves were spawned!");
            return;
        }

        EnemyWave currentWave = currentEnemyComposition.EnemyWaves[currentWaveIndex];

        List<ScriptableEnemy> enemiesInWave = GetEnemiesInWave(currentWave);
        Vector2[] enemyPositions = GetRandomPositions(enemiesInWave.Count);

        bool firstWave = currentWaveIndex == 0;
        if (firstWave) {
            SpawnEnemies(enemiesInWave);
        }
        else {
            CreateSpawnEffects(enemyPositions);
        }

        currentWaveIndex++;
    }

    private List<ScriptableEnemy> GetEnemiesInWave(EnemyWave currentWave) {
        List<ScriptableEnemy> enemies = new List<ScriptableEnemy>();

        foreach (EnemyAmount enemyAmount in currentWave.EnemyAmounts) {
            enemyAmount.Amount.Randomize();
            for (int i = 0; i < enemyAmount.Amount.Value; i++) {
                enemies.Add(enemyAmount.ScriptableEnemy);
            }
        }

        return enemies;
    }

    private Vector2[] GetRandomPositions(int amount) {
        Vector2[] enemySpawnPositions = new Vector2[amount];

        for (int i = 0; i < amount; i++) {
            float avoidRadius = 2f;
            Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.transform.position, avoidRadius);
            enemySpawnPositions[i] = position;
        }

        return enemySpawnPositions;
    }

    [SerializeField] private SpawnInEffect spawnInEffectPrefab;

    private void CreateSpawnEffects(Vector2[] positions) {
        foreach (Vector2 position in positions) {
            SpawnInEffect spawnInEffect = spawnInEffectPrefab.Spawn(position, Containers.Instance.Effects);
            spawnInEffect.Grow();
        }
    }

    private void SpawnEnemies(List<ScriptableEnemy> enemies) {
        foreach (ScriptableEnemy enemy in enemies) {
            SpawnEnemy(enemy.Prefab);
        }
    }

    private void SpawnEnemy(Enemy enemyPrefab) {
        float avoidRadius = 2f;
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.transform.position, avoidRadius);
        enemyPrefab.Spawn(position, Containers.Instance.Enemies);
    }

    private void SpawnEnemy(Enemy enemyPrefab, Vector2 pos) {
        enemyPrefab.Spawn(pos, Containers.Instance.Enemies);
    }

    public void StopSpawningInRoom() {

    }
}
