using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour, IHasRoomNum {

    [SerializeField] private ScriptableEnemy scriptableEnemy;
    [SerializeField] private float delay;

    [SerializeField] private bool randomEnemy;
    [SerializeField] private ScriptableEnemy[] scriptableEnemies;
    [SerializeField, Range(0f, 1f)] private float chanceToSpawn;

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
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds(delay);

        if (Random.value < chanceToSpawn) {
            if (randomEnemy) {
                scriptableEnemies.RandomItem().Prefab.Spawn(transform.position, Containers.Instance.Enemies);
            }
            else {
                scriptableEnemy.Prefab.Spawn(transform.position, Containers.Instance.Enemies);
            }
        }
    }
}
