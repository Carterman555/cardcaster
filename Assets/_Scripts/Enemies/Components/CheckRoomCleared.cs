using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class CheckRoomCleared : MonoBehaviour {

    public static event Action OnEnemiesCleared;
    private static bool isOnCooldown = false;

    private void OnDisable() {
        if (Helpers.GameStopping()) {
            return;
        }
        EnemySpawnerOld.Instance.StartCoroutine(CheckIfEnemiesCleared());
    }

    private IEnumerator CheckIfEnemiesCleared() {
        // wait because some enemies spawn more on death and it needs to register those
        yield return null;
        bool anyAliveEnemies = Containers.Instance.Enemies.GetComponentsInChildren<Health>().Any(health => !health.IsDead());

        if (!anyAliveEnemies && !EnemySpawner.Instance.SpawningEnemies() && !isOnCooldown) {
            isOnCooldown = true;
            OnEnemiesCleared?.Invoke();

            // cooldown so OnEnemiesCleared is only invoked once when multiple enemies die at the same time
            float cooldownDuration = 0.25f;
            yield return new WaitForSeconds(cooldownDuration);
            isOnCooldown = false;
        }
    }

    // debug
    public static void InvokeCleared() {
        if (!isOnCooldown) {
            isOnCooldown = true;
            OnEnemiesCleared?.Invoke();
            EnemySpawnerOld.Instance.StartCoroutine(ResetCooldown());
        }
    }

    private static IEnumerator ResetCooldown() {
        float cooldownDuration = 0.25f;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }
}
