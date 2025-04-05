using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour, IHasRoomNum {

    [SerializeField] private bool specificEnemy;

    [ConditionalHide("specificEnemy")]
    [SerializeField] private ScriptableEnemy enemyToSpawn;


    [ConditionalHideReversed("specificEnemy")]
    [SerializeField] private EnemyTag possibleTags;

    [ConditionalHideReversed("specificEnemy")]
    [SerializeField] private float maxDifficulty;

    private int roomNum;

    public void SetRoomNum(int roomNum) {
        this.roomNum = roomNum;
    }

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += TrySpawnEnemy;
    }
    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TrySpawnEnemy;
    }

    private void TrySpawnEnemy(Room enteredRoom) {
        if (enteredRoom.GetRoomNum() == roomNum && !enteredRoom.IsRoomCleared()) {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy() {

        Enemy enemyPrefab;

        if (specificEnemy) {
            enemyPrefab = enemyToSpawn.Prefab;
        }
        else {
            enemyPrefab = ChooseRandomEnemy();
        }

        enemyPrefab.Spawn(transform.position, Containers.Instance.Enemies);
    }

    private Enemy ChooseRandomEnemy() {
        List<ScriptableEnemy> allEnemies = ResourceSystem.Instance.GetAllEnemies();

        // Filter enemies that match any of the possible tags
        var matchingEnemies = allEnemies.Where(enemy => (enemy.Tags & possibleTags) != EnemyTag.None && enemy.Difficulty <= maxDifficulty).ToArray();

        if (matchingEnemies.Length == 0) {
            Debug.LogError("No enemies match the given tags.");
            return null;
        }


        // Choose a random enemy from the matching ones to spawn
        Enemy enemyPrefab = matchingEnemies.RandomItem().Prefab;
        return enemyPrefab;
    }
}
