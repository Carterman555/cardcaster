using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour, IHasRoomNum {

    [SerializeField] private ScriptableEnemy scriptableEnemy;
    [SerializeField] private float delay;

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
        if (enteredRoom.GetRoomNum() == roomNum) {
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds(delay);
        scriptableEnemy.Prefab.Spawn(transform.position, Containers.Instance.Enemies);
    }
}
