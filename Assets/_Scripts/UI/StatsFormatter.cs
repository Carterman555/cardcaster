using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class StatsFormatter : StaticInstance<StatsFormatter> {

    [SerializeField] private StatStrings[] statStrings;

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

    [Serializable]
    private struct StatStrings {
        public PlayerStatType StatType;
        public LocalizedString LocalizedString;
    }
}

