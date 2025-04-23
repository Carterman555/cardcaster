using QFSW.QC;
using System.Linq;
using UnityEngine;

public class EnemySpawner : StaticInstance<EnemySpawner> {

    private ScriptableEnemyComposition currentEnemyComposition;

    private int currentWaveIndex;

    public bool SpawnedAllWaves() {

        if (currentEnemyComposition == null) {
            return true;
        }

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
        if (room.IsRoomCleared) {
            return;
        }
        
        currentEnemyComposition = room.ScriptableRoom.ScriptableEnemyComposition;
        currentWaveIndex = 0;

        SpawnCurrentWave();
    }

    public void SpawnCurrentWave() {

        if (SpawnedAllWaves()) {
            Debug.LogWarning("Trying to spawn current wave, but all waves were spawned!");
            return;
        }

        EnemyWave currentWave = currentEnemyComposition.EnemyWaves[currentWaveIndex];

        foreach (EnemyAmount enemyAmount in currentWave.EnemyAmounts) {
            enemyAmount.Amount.Randomize();
            for (int i = 0; i < enemyAmount.Amount.Value; i++) {
                bool firstWave = currentWaveIndex == 0;
                SpawnEnemy(enemyAmount.ScriptableEnemy.Prefab, createSpawnEffect: !firstWave);
            }
        }

        currentWaveIndex++;
    }

    [SerializeField] private SpawnEffect spawnEffectPrefab;

    private void SpawnEnemy(Enemy enemyPrefab, bool createSpawnEffect = true) {
        Vector2 position = new RoomPositionHelper().GetRandomRoomPos(PlayerMovement.Instance.CenterPos,
            avoidRadius: 2f,
            entranceAvoidDistance: 3f);

        SpawnEnemy(enemyPrefab, position, createSpawnEffect);
    }

    public void SpawnEnemy(Enemy enemyPrefab, Vector2 position, bool createSpawnEffect = true) {

        if (createSpawnEffect) {
            SpawnEffect spawnEffect = spawnEffectPrefab.Spawn(position, Containers.Instance.Effects);
            spawnEffect.Setup(enemyPrefab);
        }
        else {
            enemyPrefab.Spawn(position, Containers.Instance.Enemies);
        }
    }

    //debug
    [Command]
    private void SpawnEnemy(string name, bool spawnEffect = false) {
        ScriptableEnemy enemy = ResourceSystem.Instance.GetAllEnemies().FirstOrDefault(e => e.name == name);
        SpawnEnemy(enemy.Prefab, spawnEffect);
    }

    public void StopSpawningInRoom() {

    }
}
