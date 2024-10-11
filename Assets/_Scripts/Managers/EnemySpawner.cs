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
        if (!room.IsRoomCleared()) {
            StartCoroutine(SpawnEnemiesCor());
        }
    }

    private IEnumerator SpawnEnemiesCor() {

        spawningEnemies = true;

        //int numOfUniqueComplexEnemies = Random.Range(0, 2);
        int numOfUniqueComplexEnemies = 1;
        Enemy[] thisRoomsComplexEnemyPrefabs = new Enemy[numOfUniqueComplexEnemies];

        for (int i = 0; i < numOfUniqueComplexEnemies; i++) {
            thisRoomsComplexEnemyPrefabs[i] = complexEnemies.RandomItem().Prefab;
        }

        spawnAmount.Randomize();
        for (int i = 0; i < spawnAmount.Value; i++) {
            yield return new WaitForSeconds(spawnCooldown.Randomize());

            if (Random.value < complexChance) {
                Enemy enemyPrefab = complexEnemies.RandomItem().Prefab;
                SpawnEnemy(enemyPrefab);
            }
            else {
                Enemy enemyPrefab = basicEnemies.RandomItem().Prefab;
                SpawnEnemy(enemyPrefab);
            }
        }

        spawningEnemies = false;
    }

    private void SpawnEnemy(Enemy enemyPrefab) {
        float avoidRadius = 2f;
        Vector2 position = GetRandomPositionInRoom(PlayerMovement.Instance.transform.position, avoidRadius);

        enemyPrefab.Spawn(position, Containers.Instance.Enemies);
    }

    private Vector2 GetRandomPositionInRoom(Vector2 avoidCenter, float avoidRadius) {
        PolygonCollider2D col = Room.GetCurrentRoom().GetComponent<PolygonCollider2D>();
        Bounds bounds = col.bounds;

        // Keep trying until we find a valid point inside the polygon, outside the no teleport circle, and on a ground tile
        Vector2 randomPoint;
        do {
            // Generate a random point within the bounds' rectangle
            randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        } while (!IsPointInPolygon(col, randomPoint) ||
        !OnlyOnGroundTile(Room.GetCurrentRoom(), randomPoint) ||
        IsPointInNoTeleportZone(randomPoint, avoidCenter, avoidRadius));

        return randomPoint;
    }

    private bool IsPointInPolygon(PolygonCollider2D col, Vector2 point) {
        return col.OverlapPoint(point);
    }

    private bool OnlyOnGroundTile(Room room, Vector2 point) {
        bool onGroundTile = OnTile(room.GetGroundTilemap(), point);
        bool onColliderTile = OnTile(room.GetColliderTilemap(), point) || OnTile(room.GetBotColliderTilemap(), point);
        return onGroundTile && !onColliderTile;
    }

    private bool OnTile(Tilemap tilemap, Vector2 point) {
        TileBase tile = tilemap.GetTile(tilemap.WorldToCell(point));
        bool onTile = tile != null;
        return onTile;
    }

    private bool IsPointInNoTeleportZone(Vector2 point, Vector2 center, float radius) {
        return Vector2.Distance(point, center) < radius;
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
}
