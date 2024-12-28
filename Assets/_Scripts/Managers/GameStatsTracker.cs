using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsTracker : StaticInstance<GameStatsTracker> {

    private int kills;

    private void OnEnable() {
        kills = 0;

        Health.OnAnyDeath += TryIncrementKills;
    }

    private void OnDisable() {
        Health.OnAnyDeath -= TryIncrementKills;
    }

    private void TryIncrementKills(Health health) {
        if (health.TryGetComponent(out Enemy enemy)) {
            kills++;
        }
    }

    #region Get Methods

    public int GetKills() {
        return kills;
    }

    #endregion
}
