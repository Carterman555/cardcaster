using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour, IHasRoomNum {

    [SerializeField] private EnemyTag possibleTags;
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

        List<ScriptableEnemy> allEnemies = ResourceSystem.Instance.GetAllEnemies();

        // Filter enemies that match any of the possible tags
        var matchingEnemies = allEnemies.Where(enemy => (enemy.Tags & possibleTags) != EnemyTag.None && enemy.Difficulty <= maxDifficulty).ToArray();

        if (matchingEnemies.Length == 0) {
            Debug.LogWarning("No enemies match the given tags.");
            return;
        }


        // Choose a random enemy from the matching ones to spawn
        var enemy = matchingEnemies.RandomItem();
        enemy.Prefab.Spawn(transform.position, Containers.Instance.Enemies);
    }
}
