using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatsManager : StaticInstance<StatsManager> {

    [SerializeField] private ScriptablePlayer scriptablePlayer;

    private PlayerStats playerStats;

    private PlayerStatsModifier allPlayerStatsModifiers;

    protected override void Awake() {
        base.Awake();

        playerStats = Instantiate(scriptablePlayer).Stats;
        allPlayerStatsModifiers = PlayerStatsModifier.Zero;
    }

    public void AddPlayerStatsModifier(PlayerStatsModifier modifier) {
        allPlayerStatsModifiers += modifier;
        UpdatePlayerStats();
    }

    public void RemovePlayerStatsModifier(PlayerStatsModifier modifier) {
        allPlayerStatsModifiers -= modifier;
        UpdatePlayerStats();
    }

    private void UpdatePlayerStats() {
        playerStats = Instantiate(scriptablePlayer).Stats;
        playerStats.ApplyModifier(allPlayerStatsModifiers); 
    }

    public PlayerStats GetPlayerStats() {
        return playerStats;
    }
}
