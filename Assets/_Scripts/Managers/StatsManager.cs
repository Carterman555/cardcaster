using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using static Mono.CSharp.Parameter;

public class StatsManager : StaticInstance<StatsManager> {

    [SerializeField] private ScriptablePlayer scriptablePlayer;
    private PlayerStats playerStats;

    private List<PlayerStatModifier> statModifiers;

    protected override void Awake() {
        base.Awake();
        statModifiers = new();
        UpdatePlayerStats();
        print("Clear stat modifiers");
    }

    private void OnEnable() {
        // Prevents accidental modification of scriptable objects in Inspector during play
        hideFlags = HideFlags.NotEditable;
    }

    public void AddPlayerStatModifiers(PlayerStatModifier[] modifiers) {
        foreach (PlayerStatModifier modifier in modifiers) {
            AddPlayerStatModifier(modifier);
        }
    }

    public void AddPlayerStatModifier(PlayerStatModifier modifier) {
        statModifiers.Add(modifier);
        UpdatePlayerStats();
    }

    public void RemovePlayerStatModifiers(PlayerStatModifier[] modifiers) {
        foreach (PlayerStatModifier modifier in modifiers) {
            RemovePlayerStatModifier(modifier);
        }
    }

    public void RemovePlayerStatModifier(PlayerStatModifier modifier) {
        if (!TryFindPlayerStatModifier(modifier, out PlayerStatModifier modifierInList)) {
            Debug.LogError("Trying to remove player stat modifer that is not in list!");
            return;
        }

        statModifiers.Remove(modifierInList);
        UpdatePlayerStats();
    }

    private bool TryFindPlayerStatModifier(PlayerStatModifier originalModifier, out PlayerStatModifier modifierInList) {
        foreach (PlayerStatModifier modifier in statModifiers) {
            if (modifier.PlayerStatType == originalModifier.PlayerStatType &&
                modifier.ModifyType == originalModifier.ModifyType &&
                modifier.Value == originalModifier.Value) {
                modifierInList = modifier;
                return true;
            }
        }

        modifierInList = default;
        return false;
    }

    private void UpdatePlayerStats() {

        playerStats = scriptablePlayer.PlayerStats;

        // two forloops to add additive modifiers first
        foreach (PlayerStatModifier statModifier in statModifiers) {
            if (statModifier.ModifyType == ModifyType.Additive) {
                ModifyStat(playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
            }
        }

        foreach (PlayerStatModifier statModifier in statModifiers) {
            if (statModifier.ModifyType == ModifyType.Multiplicative) {
                ModifyStat(playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
            }
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
