using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsTracker : StaticInstance<GameStatsTracker> {

    private int kills;

    private void OnEnable() {
        kills = 0;

        EnemyHealth.OnAnyDeath += IncrementKills;
    }

    private void OnDisable() {
        EnemyHealth.OnAnyDeath -= IncrementKills;
    }

    private void IncrementKills(EnemyHealth health) {
        kills++;
    }

    #region Get Methods

    public int GetKills() {
        return kills;
    }

    #endregion
}
