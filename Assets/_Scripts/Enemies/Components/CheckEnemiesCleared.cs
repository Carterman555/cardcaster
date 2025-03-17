using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class CheckEnemiesCleared : MonoBehaviour {

    public static event Action OnEnemiesCleared;
    private static bool isOnCooldown = false;

    private void OnDisable() {
        if (Helpers.GameStopping() || GameSceneManager.Instance.IsSceneLoading()) {
            return;
        }

        //... needs outside behaviour to start it because this is disabled
        EnemySpawner.Instance.StartCoroutine(CheckIfEnemiesCleared());
    }

    private IEnumerator CheckIfEnemiesCleared() {
        //... wait because some enemies spawn more on death and it needs to register those
        yield return null;
        bool anyAliveEnemies = Containers.Instance.Enemies.GetComponentsInChildren<Health>().Any(health => !health.Dead);

        if (!anyAliveEnemies && !isOnCooldown) {

            //... if the room was cleared
            if (EnemySpawner.Instance.SpawnedAllWaves()) {
                OnEnemiesCleared?.Invoke();
            }
            //... if another wave
            else {
                EnemySpawner.Instance.SpawnCurrentWave();
            }

            // cooldown so OnEnemiesCleared is only invoked once when multiple enemies die at the same time
            isOnCooldown = true;
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
            EnemySpawner.Instance.StartCoroutine(ResetCooldown());
        }
    }

    private static IEnumerator ResetCooldown() {
        float cooldownDuration = 0.25f;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }
}
