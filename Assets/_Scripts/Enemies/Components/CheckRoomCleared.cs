using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckRoomCleared : MonoBehaviour {

    public static event Action OnEnemiesCleared;

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();

        health.OnDeath += OnDeath;
    }

    private void OnDestroy() {
        health.OnDeath -= OnDeath;
    }

    private void OnDeath() {
        CheckIfEnemiesCleared();

        DeckManager.Instance.IncreaseEssence(1f);
    }



    private void CheckIfEnemiesCleared() {
        bool anyAliveEnemies = Containers.Instance.Enemies.GetComponentsInChildren<Health>().Any(health => !health.IsDead());
        if (!anyAliveEnemies) {
            OnEnemiesCleared?.Invoke();
        }
    }
}
