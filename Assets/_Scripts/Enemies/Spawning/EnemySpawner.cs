using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : StaticInstance<EnemySpawner> {

    private bool spawningEnemies;

    private ScriptableEnemyComposition currentEnemyComposition;

    private List<Enemy> enemiesLeftToSpawn;

    public bool SpawningEnemies() {
        return spawningEnemies;
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

        SpawnInitialEnemies();

        SetEnemiesToSpawnList();
        StartCoroutine(SpawnTimedEnemiesCor());
    }

    private void SpawnInitialEnemies() {
        foreach (EnemyAmount enemyAmount in currentEnemyComposition.InitialEnemyAmounts) {
            enemyAmount.Amount.Randomize();
            for (int i = 0; i < enemyAmount.Amount.Value; i++) {
                SpawnEnemy(enemyAmount.ScriptableEnemy.Prefab);
            }
        }
    }

    private void SetEnemiesToSpawnList() {
        enemiesLeftToSpawn.Clear();

        foreach (EnemyAmount enemyAmount in currentEnemyComposition.TimedEnemyAmounts) {
            int chosenAmount = enemyAmount.Amount.Randomize();
            for (int i = 0; i < chosenAmount; i++) {
                enemiesLeftToSpawn.Add(enemyAmount.ScriptableEnemy.Prefab);
            }
        }
    }

    private IEnumerator SpawnTimedEnemiesCor() {

        spawningEnemies = true;

        yield return new WaitForSeconds(currentEnemyComposition.AfterInitialEnemiesDelay.Randomize());

        while (enemiesLeftToSpawn.Count > 0) {
            yield return new WaitForSeconds(currentEnemyComposition.BetweenEnemyDelay.Randomize());

            Enemy randomEnemy
            SpawnEnemy();
        }


        spawningEnemies = false;
    }

    // chooses random enemy to spawn taking into account how much they are weighted
    private void SpawnRandomEnemy() {

        float probabilitySum = 0;
        foreach (EnemyAmount enemyProbability in currentEnemyComposition) {
            probabilitySum += enemyProbability.Probability;
        }

        float randomNum = UnityEngine.Random.Range(0f, probabilitySum);

        foreach (EnemyAmount enemyProbability in currentEnemyComposition) {
            randomNum -= enemyProbability.Probability;
            if (randomNum < 0f) {

                Enemy randomEnemyPrefab = enemyProbability.ScriptableEnemy.Prefab;
                SpawnEnemy(randomEnemyPrefab);
                return;
            }
        }
    }

    private void SpawnEnemy(Enemy enemyPrefab) {
        float avoidRadius = 2f;
        Vector2 position = new RoomPositionHelper().GetRandomSpawnPos(PlayerMovement.Instance.transform.position, avoidRadius);
        enemyPrefab.Spawn(position, Containers.Instance.Enemies);
    }
}
