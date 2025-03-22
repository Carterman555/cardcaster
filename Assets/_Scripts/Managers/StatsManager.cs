using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Reflection;


public class StatsManager : StaticInstance<StatsManager> {

    [SerializeField] private ScriptablePlayer scriptablePlayer;
    private PlayerStats playerStats;

    private List<PlayerStatsModifier> statsModifiers;

    protected override void Awake() {
        base.Awake();
        statsModifiers = new();
        print("Clear stat modifiers");
    }

    private void OnEnable() {
        // Prevents accidental modification of scriptable objects in Inspector during play
        hideFlags = HideFlags.NotEditable;
    }

    public void AddPlayerStatsModifier(PlayerStatsModifier modifier) {

        if (statsModifiers.Any(s => s.ID == modifier.ID)) {
            Debug.LogError("Trying to add modifier that was already added!");
            return;
        }

        statsModifiers.Add(modifier);
        modifier.ID = Guid.NewGuid().ToString();

        UpdatePlayerStats();
    }

    public void RemovePlayerStatsModifier(PlayerStatsModifier modifier) {

        PlayerStatsModifier modifierToRemove = statsModifiers.FirstOrDefault(s => s.ID == modifier.ID);
        if (modifierToRemove == null) {
            Debug.LogError("Trying to remove modifier that is not in list!");
            return;
        }

        statsModifiers.Remove(modifierToRemove);

        UpdatePlayerStats();
    }

    private readonly List<PlayerStatModifier> additiveModifiers = new();
    private readonly List<PlayerStatModifier> multiplictiveModifiers = new();

    private void UpdatePlayerStats() {

        additiveModifiers.Clear();
        multiplictiveModifiers.Clear();

        foreach (PlayerStatsModifier playerStatsModifier in statsModifiers) {
            additiveModifiers.AddRange(playerStatsModifier.StatModifiers.Where(m => m.ModifyType == ModifyType.Additive));
            multiplictiveModifiers.AddRange(playerStatsModifier.StatModifiers.Where(m => m.ModifyType == ModifyType.Multiplicative));
        }

        playerStats = scriptablePlayer.PlayerStats;

        foreach (PlayerStatModifier statModifier in additiveModifiers) {
            ModifyStat(playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
        }

        foreach (PlayerStatModifier statModifier in multiplictiveModifiers) {
            ModifyStat(playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
        }

        playerStats.MaxHealth = Mathf.Max(playerStats.MaxHealth, 1f);
        playerStats.KnockbackResistance = Mathf.Max(playerStats.KnockbackResistance, 0.1f);
        playerStats.MoveSpeed = Mathf.Max(playerStats.MoveSpeed, 0f);
        playerStats.Damage = Mathf.Max(playerStats.Damage, 0f);
        playerStats.AttackSpeed = Mathf.Max(playerStats.AttackSpeed, 0.1f);
        playerStats.KnockbackStrength = Mathf.Max(playerStats.KnockbackStrength, 0f);
        playerStats.SwordSize = Mathf.Max(playerStats.SwordSize, 0f);
        playerStats.DashSpeed = Mathf.Max(playerStats.DashSpeed, 0f);
        playerStats.DashDistance = Mathf.Max(playerStats.DashDistance, 0f);
        playerStats.DashAttackDamage = Mathf.Max(playerStats.DashAttackDamage, 0f);
        playerStats.DashRechargeSpeed = Mathf.Max(playerStats.DashRechargeSpeed, 0f);
        playerStats.CritChance = Mathf.Max(playerStats.CritChance, 0f, 1f);
        playerStats.CritDamageMult = Mathf.Max(playerStats.CritDamageMult, 0f);
        playerStats.ProjectileDamageMult = Mathf.Max(playerStats.ProjectileDamageMult, 0f);
        playerStats.AllDamageMult = Mathf.Max(playerStats.AllDamageMult, 0f);
        playerStats.MaxEssence = (int)Mathf.Max(playerStats.MaxEssence, 0f);
        playerStats.HandSize = (int)Mathf.Max(playerStats.HandSize, 0f);

        print(playerStats.Damage);
    }

    public PlayerStats GetPlayerStats() {
        return playerStats;
    }


    public void ModifyStat(PlayerStats playerStats, PlayerStatType playerStatType, ModifyType modifyType, float value) {
        Type type = typeof(PlayerStats);
        FieldInfo field = type.GetField(playerStatType.ToString());

        if (field != null) {
            float currentValue = (float)field.GetValue(playerStats);

            if (modifyType == ModifyType.Additive) {
                field.SetValue(playerStats, currentValue + value);
            }
            else if (modifyType == ModifyType.Multiplicative) {
                field.SetValue(playerStats, currentValue * value);
            }
        }
        else {
            throw new ArgumentException($"Stat {playerStatType} does not exist in PlayerStats.");
        }
    }

}
