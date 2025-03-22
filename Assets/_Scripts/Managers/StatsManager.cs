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

    private List<PlayerStatsModifier> statsModifiers;

    protected override void Awake() {
        base.Awake();
        statsModifiers = new();
        print("Clear stat modifiers");
    }

    public void AddPlayerStatsModifier(PlayerStatsModifier modifier) {

        if (statsModifiers.Any(s => s.ID == modifier.ID)) {
            Debug.LogError("Trying to add modifier that was already added!");
            return;
        }

        statsModifiers.Add(modifier);
        modifier.ID = Guid.NewGuid().ToString();
    }

    public void RemovePlayerStatsModifier(PlayerStatsModifier modifier) {

        if (statsModifiers.Any(m => m.ID == modifier.ID)) {
            Debug.LogError("Trying to remove modifier that is not in list!");
            return;
        }

        PlayerStatsModifier modifierToRemove = statsModifiers.FirstOrDefault(m => m.ID == modifier.ID);
        statsModifiers.Remove(modifierToRemove);
    }

    private readonly List<PlayerStatModifier> additiveModifiers = new();
    private readonly List<PlayerStatModifier> multiplictiveModifiers = new();

    public PlayerStats GetPlayerStats() {

        additiveModifiers.Clear();
        multiplictiveModifiers.Clear();

        foreach (PlayerStatsModifier playerStatsModifier in statsModifiers) {
            additiveModifiers.AddRange(playerStatsModifier.StatModifiers.Where(m => m.ModifyType == ModifyType.Additive));
            multiplictiveModifiers.AddRange(playerStatsModifier.StatModifiers.Where(m => m.ModifyType == ModifyType.Multiplicative));
        }

        PlayerStats playerStats = scriptablePlayer.BaseStats;

        foreach (PlayerStatModifier statModifier in additiveModifiers) {
            ModifyStat(playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
        }

        foreach (PlayerStatModifier statModifier in multiplictiveModifiers) {
            ModifyStat(playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
        }

        playerStats.CommonStats.MaxHealth = Mathf.Max(playerStats.CommonStats.MaxHealth, 1f);
        playerStats.CommonStats.KnockbackResistance = Mathf.Max(playerStats.CommonStats.KnockbackResistance, 0.1f);
        playerStats.CommonStats.MoveSpeed = Mathf.Max(playerStats.CommonStats.MoveSpeed, 0f);
        playerStats.CommonStats.Damage = Mathf.Max(playerStats.CommonStats.Damage, 0f);
        playerStats.CommonStats.AttackSpeed = Mathf.Max(playerStats.CommonStats.AttackSpeed, 0.1f);
        playerStats.CommonStats.KnockbackStrength = Mathf.Max(playerStats.CommonStats.KnockbackStrength, 0f);
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

        print(playerStats.CommonStats.Damage);

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
