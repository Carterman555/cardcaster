using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class StatsText : MonoBehaviour {

    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += UpdateText;

        UpdateText(null);
    }

    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateText;
    }

    private void UpdateText(Locale locale) {
        PlayerStats playerStats = StatsManager.PlayerStats;

        text.text = $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.MaxHealth, playerStats.MaxHealth)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.KnockbackResistance, playerStats.KnockbackResistance)}\n\n" +

            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.MoveSpeed, playerStats.MoveSpeed)}\n\n" +

            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.BasicAttackDamage, playerStats.BasicAttackDamage)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.AttackSpeed, playerStats.AttackSpeed)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.KnockbackStrength, playerStats.KnockbackStrength)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.SwordSize, playerStats.SwordSize)}\n\n" +

            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.DashDistance, playerStats.DashDistance)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.DashAttackDamage, playerStats.DashAttackDamage)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.DashRechargeSpeed, playerStats.DashRechargeSpeed)}\n\n" +

            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.CritChance, playerStats.CritChance)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.CritDamageMult, playerStats.CritDamageMult)}\n\n" +

            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.ProjectileDamageMult, playerStats.ProjectileDamageMult)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.AllDamageMult, playerStats.AllDamageMult)}\n\n" +

            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.MaxEssence, playerStats.MaxEssence)}\n" +
            $"{StatsFormatter.Instance.GetStatStr(PlayerStatType.HandSize, playerStats.HandSize)}";
    }
}
