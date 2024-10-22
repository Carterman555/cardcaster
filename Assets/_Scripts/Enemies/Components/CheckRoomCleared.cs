using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckRoomCleared : MonoBehaviour {

    public static event Action OnEnemiesCleared;

    private void OnDisable() {
        EnemySpawner.Instance.StartCoroutine(CheckIfEnemiesCleared());
    }

    private IEnumerator CheckIfEnemiesCleared() {

        // wait because some enemies spawn more on death and it needs to register those
        yield return null;

        bool anyAliveEnemies = Containers.Instance.Enemies.GetComponentsInChildren<Health>().Any(health => !health.IsDead());
        if (!anyAliveEnemies && !EnemySpawner.Instance.SpawningEnemies()) {
            OnEnemiesCleared?.Invoke();
        }
    }
}
