using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : StaticInstance<EnemySpawner> {

    [SerializeField] private ScriptableEnemy[] basicEnemies;
    [SerializeField] private ScriptableEnemy[] complexEnemies;

    [SerializeField][Range(0f, 1f)] private float complexChance;

    [SerializeField] private RandomInt spawnAmount;
    [SerializeField] private RandomFloat spawnCooldown;

    private bool spawningEnemies;

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

        StartCoroutine(SpawnEnemiesCor());
    }

    private IEnumerator SpawnEnemiesCor() {

        spawningEnemies = true;

        int numOfUniqueComplexEnemies = Random.Range(1, 3); // 1 or 2
        Enemy[] thisRoomsComplexEnemyPrefabs = new Enemy[numOfUniqueComplexEnemies];

        for (int i = 0; i < numOfUniqueComplexEnemies; i++) {
            thisRoomsComplexEnemyPrefabs[i] = complexEnemies.RandomItem().Prefab;
        }

        spawnAmount.Randomize();
        for (int i = 0; i < spawnAmount.Value; i++) {
            yield return new WaitForSeconds(spawnCooldown.Randomize());

            if (Random.value < complexChance) {
                ScriptableEnemy scriptableEnemy = complexEnemies.RandomItem();
                Enemy enemyPrefab = scriptableEnemy.Prefab;

                if (enemyPrefab == null) {
                    print($"Null: {scriptableEnemy.Name}");
                }

                SpawnEnemy(enemyPrefab);
            }
            else {
                ScriptableEnemy scriptableEnemy = basicEnemies.RandomItem();
                Enemy enemyPrefab = scriptableEnemy.Prefab;

                if (enemyPrefab == null) {
                    print($"Null: {scriptableEnemy.Name}");
                }

                SpawnEnemy(enemyPrefab);
            }
        }

        spawningEnemies = false;
    }

    private void SpawnEnemy(Enemy enemyPrefab) {
        float avoidRadius = 2f;
        Vector2 position = new RoomPositionHelper().GetRandomSpawnPos(PlayerMovement.Instance.transform.position, avoidRadius);
        enemyPrefab.Spawn(position, Containers.Instance.Enemies);
    }

    // debugging
    [Command]
    private void SpawnEnemy(string enemyName) {
        ScriptableEnemy enemy = ResourceSystem.Instance.GetAllEnemies().FirstOrDefault(e => e.name == enemyName);

        if (enemy == null) {
            Debug.LogWarning("Couldn't find enemy by name");
            return;
        }

        SpawnEnemy(enemy.Prefab);
    }

    public void StopSpawning() {
        StopAllCoroutines();
        spawningEnemies = false;
    }
}
