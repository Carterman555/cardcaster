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
        Room.OnAnyRoomChange += TrySpawnEnemy;
    }
    private void OnDisable() {
        Room.OnAnyRoomChange -= TrySpawnEnemy;
    }

    private void TrySpawnEnemy(int enteredRoomNum) {
        if (enteredRoomNum == roomNum) {
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy() {
        yield return new WaitForSeconds(delay);
        scriptableEnemy.Prefab.Spawn(transform.position, Containers.Instance.Enemies);
    }
}
