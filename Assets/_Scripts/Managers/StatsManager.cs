using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {

    public static event Action<PlayerStatType> OnStatsChanged;

    private static ScriptablePlayer scriptablePlayer;

    private static PlayerStats playerStats;
    public static PlayerStats PlayerStats => playerStats;

    private static List<PlayerStatModifier> statModifiers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        scriptablePlayer = Resources.Load<ScriptablePlayer>("Player");
        ClearStats();
    }

    private void OnEnable() {
        GameSceneManager.OnStartGameLoadingStarted += ClearStats;
    }

    private void OnDisable() {
        GameSceneManager.OnStartGameLoadingStarted -= ClearStats;
    }

    public static void ClearStats() {
        statModifiers = new();
        UpdatePlayerStats();
    }

    public static void AddPlayerStatModifiers(PlayerStatModifier[] modifiers) {
        foreach (PlayerStatModifier modifier in modifiers) {
            AddPlayerStatModifier(modifier);
        }
    }

    public static void AddPlayerStatModifier(PlayerStatModifier modifier) {
        statModifiers.Add(modifier);
        UpdatePlayerStats();

        OnStatsChanged?.Invoke(modifier.PlayerStatType);
    }

    public static void RemovePlayerStatModifiers(PlayerStatModifier[] modifiers) {
        foreach (PlayerStatModifier modifier in modifiers) {
            RemovePlayerStatModifier(modifier);
        }
    }

    public static void RemovePlayerStatModifier(PlayerStatModifier modifier) {
        if (!TryFindPlayerStatModifier(modifier, out PlayerStatModifier modifierInList)) {
            Debug.LogError("Trying to remove player stat modifer that is not in list!");
            return;
        }

        statModifiers.Remove(modifierInList);
        UpdatePlayerStats();

        OnStatsChanged?.Invoke(modifier.PlayerStatType);
    }

    private static bool TryFindPlayerStatModifier(PlayerStatModifier originalModifier, out PlayerStatModifier modifierInList) {
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

    private static void UpdatePlayerStats() {

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
        playerStats.HandSize = (int)Mathf.Clamp(playerStats.HandSize, 0f, DeckManager.MaxHandSize);
    }

    public static void ModifyStat(ref PlayerStats playerStats, PlayerStatType playerStatType, ModifyType modifyType, float value) {
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
            case PlayerStatType.DashDistance:
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

    private static void ApplyModification(ref float stat, ModifyType modifyType, float value) {
        if (modifyType == ModifyType.Additive) {
            stat += value;
        }
        else if (modifyType == ModifyType.Multiplicative) {
            stat *= value;
        }
    }

    private static void ApplyModification(ref int stat, ModifyType modifyType, int value) {
        if (modifyType == ModifyType.Additive) {
            stat += value;
        }
        else if (modifyType == ModifyType.Multiplicative) {
            stat *= value;
        }
    }

    // debugging
    [ContextMenu("Print Stats")]
    public void PrintStats() {
        Debug.Log($"MaxHealth: {playerStats.MaxHealth}\n" +
                  $"KnockbackResistance: {playerStats.KnockbackResistance}\n" +
                  $"MoveSpeed: {playerStats.MoveSpeed}\n" +
                  $"BaseBasicAttackDamage: {playerStats.BaseBasicAttackDamage}\n" +
                  $"AttackSpeed: {playerStats.AttackSpeed}\n" +
                  $"AttackCooldown: {playerStats.AttackCooldown}\n" +
                  $"KnockbackStrength: {playerStats.KnockbackStrength}\n" +
                  $"SwordSize: {playerStats.SwordSize}\n" +
                  $"DashSpeed: {playerStats.DashSpeed}\n" +
                  $"DashDistance: {playerStats.DashDistance}\n" +
                  $"BaseDashAttackDamage: {playerStats.BaseDashAttackDamage}\n" +
                  $"DashRechargeSpeed: {playerStats.DashRechargeSpeed}\n" +
                  $"DashCooldown: {playerStats.DashCooldown}\n" +
                  $"CritChance: {playerStats.CritChance}\n" +
                  $"CritDamageMult: {playerStats.CritDamageMult}\n" +
                  $"BaseProjectileDamageMult: {playerStats.BaseProjectileDamageMult}\n" +
                  $"AllDamageMult: {playerStats.AllDamageMult}\n" +
                  $"BasicAttackDamage: {playerStats.BasicAttackDamage}\n" +
                  $"DashAttackDamage: {playerStats.DashAttackDamage}\n" +
                  $"ProjectileDamageMult: {playerStats.ProjectileDamageMult}\n" +
                  $"MaxEssence: {playerStats.MaxEssence}\n" +
                  $"HandSize: {playerStats.HandSize}");
    }
}
