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
        if (statModifier.Value >= 0) {
            modifierStr += $"<color=green>+{statModifier.Value.ToString()}</color>";
        }
        else if (statModifier.Value < 0) {
            modifierStr += $"<color=red>{statModifier.Value.ToString()}</color>";
        }

        string statTypeStr = statStrings.FirstOrDefault(str => str.StatType == statModifier.PlayerStatType).LocalizedString.GetLocalizedString();
        modifierStr += $" {statTypeStr}";

        return modifierStr;
    }

    [Serializable]
    private struct StatStrings {
        public PlayerStatType StatType;
        public LocalizedString LocalizedString;
    }
}

