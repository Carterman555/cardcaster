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
    public PlayerStats PlayerStats => playerStats;

    private List<PlayerStatModifier> statModifiers;

    protected override void Awake() {
        base.Awake();
        statModifiers = new();
        UpdatePlayerStats();
        print("Clear stat modifiers");
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
                ModifyStat(ref playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
            }
        }

        foreach (PlayerStatModifier statModifier in statModifiers) {
            if (statModifier.ModifyType == ModifyType.Multiplicative) {
                ModifyStat(ref playerStats, statModifier.PlayerStatType, statModifier.ModifyType, statModifier.Value);
            }
        }

        playerStats.MaxHealth = Mathf.Max(playerStats.MaxHealth, 1f);
        playerStats.KnockbackResistance = Mathf.Max(playerStats.KnockbackResistance, 0.1f);
        playerStats.MoveSpeed = Mathf.Max(playerStats.MoveSpeed, 0f);
        playerStats.BaseBasicAttackDamage = Mathf.Max(playerStats.BaseBasicAttackDamage, 0f);
        playerStats.AttackSpeed = Mathf.Max(playerStats.AttackSpeed, 0.1f);
        playerStats.KnockbackStrength = Mathf.Max(playerStats.KnockbackStrength, 0f);
        playerStats.SwordSize = Mathf.Max(playerStats.SwordSize, 0f);
        playerStats.DashSpeed = Mathf.Max(playerStats.DashSpeed, 0f);
        playerStats.DashDistance = Mathf.Max(playerStats.DashDistance, 0f);
        playerStats.BaseDashAttackDamage = Mathf.Max(playerStats.BaseDashAttackDamage, 0f);
        playerStats.DashRechargeSpeed = Mathf.Max(playerStats.DashRechargeSpeed, 0f);
        playerStats.CritChance = Mathf.Clamp(playerStats.CritChance, 0f, 1f);
        playerStats.CritDamageMult = Mathf.Max(playerStats.CritDamageMult, 0f);
        playerStats.BaseProjectileDamageMult = Mathf.Max(playerStats.BaseProjectileDamageMult, 0f);
        playerStats.AllDamageMult = Mathf.Max(playerStats.AllDamageMult, 0f);
        playerStats.MaxEssence = (int)Mathf.Max(playerStats.MaxEssence, 0f);
        playerStats.HandSize = (int)Mathf.Max(playerStats.HandSize, 0f);
    }

    public void ModifyStat(ref PlayerStats playerStats, PlayerStatType playerStatType, ModifyType modifyType, float value) {
        switch (playerStatType) {
            case PlayerStatType.MaxHealth:
                ApplyModification(ref playerStats.MaxHealth, modifyType, value);
                break;
            case PlayerStatType.KnockbackResistance:
                ApplyModification(ref playerStats.KnockbackResistance, modifyType, value);
                break;
            case PlayerStatType.MoveSpeed:
                ApplyModification(ref playerStats.MoveSpeed, modifyType, value);
                break;
            case PlayerStatType.Damage:
                ApplyModification(ref playerStats.BaseBasicAttackDamage, modifyType, value);
                break;
            case PlayerStatType.AttackSpeed:
                ApplyModification(ref playerStats.AttackSpeed, modifyType, value);
                break;
            case PlayerStatType.KnockbackStrength:
                ApplyModification(ref playerStats.KnockbackStrength, modifyType, value);
                break;
            case PlayerStatType.SwordSize:
                ApplyModification(ref playerStats.SwordSize, modifyType, value);
                break;
            case PlayerStatType.DashSpeed:
                ApplyModification(ref playerStats.DashSpeed, modifyType, value);
                break;
            case PlayerStatType.DashTime:
                ApplyModification(ref playerStats.DashDistance, modifyType, value);
                break;
            case PlayerStatType.DashAttackDamage:
                ApplyModification(ref playerStats.BaseDashAttackDamage, modifyType, value);
                break;
            case PlayerStatType.DashRechargeSpeed:
                ApplyModification(ref playerStats.DashRechargeSpeed, modifyType, value);
                break;
            case PlayerStatType.CritChance:
                ApplyModification(ref playerStats.CritChance, modifyType, value);
                break;
            case PlayerStatType.CritDamageMult:
                ApplyModification(ref playerStats.CritDamageMult, modifyType, value);
                break;
            case PlayerStatType.ProjectileDamageMult:
                ApplyModification(ref playerStats.BaseProjectileDamageMult, modifyType, value);
                break;
            case PlayerStatType.AllDamageMult:
                ApplyModification(ref playerStats.AllDamageMult, modifyType, value);
                break;
            case PlayerStatType.MaxEssence:
                ApplyModification(ref playerStats.MaxEssence, modifyType, Mathf.RoundToInt(value));
                break;
            case PlayerStatType.HandSize:
                ApplyModification(ref playerStats.HandSize, modifyType, Mathf.RoundToInt(value));
                break;
        }
    }

    private void ApplyModification(ref float stat, ModifyType modifyType, float value) {
        if (modifyType == ModifyType.Additive) {
            stat += value;
        }
        else if (modifyType == ModifyType.Multiplicative) {
            stat *= value;
        }
    }

    private void ApplyModification(ref int stat, ModifyType modifyType, int value) {
        if (modifyType == ModifyType.Additive) {
            stat += value;
        }
        else if (modifyType == ModifyType.Multiplicative) {
            stat *= value;
        }
    }
}
