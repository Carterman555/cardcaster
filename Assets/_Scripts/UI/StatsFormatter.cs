using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class StatsFormatter : StaticInstance<StatsFormatter> {

    [SerializeField] private StatStrings[] statStrings;

    private Dictionary<PlayerStatType, StatFormatType> statFormatTypes;

    protected override void Awake() {
        base.Awake();

        statFormatTypes = new() {
            { PlayerStatType.MaxHealth, StatFormatType.Normal },
            { PlayerStatType.KnockbackResistance, StatFormatType.Normal },
            { PlayerStatType.MoveSpeed, StatFormatType.Normal },
            { PlayerStatType.BasicAttackDamage, StatFormatType.Normal },
            { PlayerStatType.AttackSpeed, StatFormatType.Normal },
            { PlayerStatType.KnockbackStrength, StatFormatType.Normal },
            { PlayerStatType.SwordSize, StatFormatType.Percent },
            { PlayerStatType.DashDistance, StatFormatType.Normal },
            { PlayerStatType.DashAttackDamage, StatFormatType.Normal },
            { PlayerStatType.DashRechargeSpeed, StatFormatType.Normal },
            { PlayerStatType.CritChance, StatFormatType.Percent },
            { PlayerStatType.CritDamageMult, StatFormatType.Percent },
            { PlayerStatType.ProjectileDamageMult, StatFormatType.Percent },
            { PlayerStatType.AllDamageMult, StatFormatType.Percent },
            { PlayerStatType.MaxEssence, StatFormatType.Normal },
            { PlayerStatType.HandSize, StatFormatType.Normal }
        };
    }

    public string GetStatStr(PlayerStatType playerStatType, float value) {
        StatFormatType statFormatType = statFormatTypes[playerStatType];

        string statTypeStr = statStrings.FirstOrDefault(str => str.StatType == playerStatType).LocalizedString.GetLocalizedString();

        if (statFormatType == StatFormatType.Normal) {
            return $"{statTypeStr}: {RoundToTenth(value)}";
        }
        else if (statFormatType == StatFormatType.Percent) {
            return $"{statTypeStr}: {Mathf.Round(value * 100)}%";
        }
        else {
            Debug.LogError($"GetStatStr cannot handle stat format type {statFormatType}!");
            return string.Empty;
        }
    }

    public string GetStatModifierStr(PlayerStatModifier statModifier) {

        string modifierStr = "";
        if (statModifier.ModifyType == ModifyType.Additive) {
            if (statModifier.Value >= 0) {
                modifierStr += $"<color=green>+{statModifier.Value.ToString()}</color>";
            }
            else if (statModifier.Value < 0) {
                modifierStr += $"<color=red>{statModifier.Value.ToString()}</color>";
            }
        }
        else if (statModifier.ModifyType == ModifyType.Multiplicative) {
            if (statModifier.Value >= 1) {
                modifierStr += $"<color=green>*{statModifier.Value.ToString()}</color>";
            }
            else if (statModifier.Value < 1) {
                modifierStr += $"<color=red>*{statModifier.Value.ToString()}</color>";
            }
        }
        else {
            Debug.LogError($"StatsFormatter cannot handle modifyType {statModifier.ModifyType}!");
        }

        string statTypeStr = statStrings.FirstOrDefault(str => str.StatType == statModifier.PlayerStatType).LocalizedString.GetLocalizedString();
        if (statTypeStr == null) {
            Debug.LogError($"Player stat type {statModifier.PlayerStatType} is not in serialized array!");
        }


        modifierStr += $" {statTypeStr}";
        return modifierStr;
    }

    public string GetStatModifiersStr(PlayerStatModifier[] statModifiers) {
        string str = "";
        for (int i = 0; i < statModifiers.Length; i++) {
            str += GetStatModifierStr(statModifiers[i]);

            bool lastStat = i == statModifiers.Length - 1;
            if (!lastStat) {
                str += "\n";
            }
        }
        return str;
    }

    private float RoundToTenth(float value) {
        return Mathf.Round(value * 10f) / 10f;
    }

    [Serializable]
    private struct StatStrings {
        public PlayerStatType StatType;
        public LocalizedString LocalizedString;
    }

    private enum StatFormatType { Normal, Percent }
}

