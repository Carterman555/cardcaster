using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Unit/Player")]
public class ScriptablePlayer : ScriptableObject {

    [field: SerializeField] public PlayerStats Stats { get; private set; }
}

[Serializable]
public class Stats {
    public float MaxHealth;
    public float KnockbackResistance;

    public float MoveSpeed;

    public float Damage;
    public float AttackSpeed;
    public float KnockbackStrength;
}

[Serializable]
public class PlayerStats : Stats {
    public float SwordSize;

    public float DashSpeed;
    public float DashTime;

    public void ApplyModifier(PlayerStatsModifier modifier) {
        MaxHealth *= PercentToMult(modifier.MaxHealthPercent);
        KnockbackResistance *= PercentToMult(modifier.KnockbackResistancePercent);
        MoveSpeed *= PercentToMult(modifier.MoveSpeedPercent);
        Damage *= PercentToMult(modifier.DamageIncreasePercent);
        AttackSpeed *= PercentToMult(modifier.AttackSpeedPercent);
        KnockbackStrength *= PercentToMult(modifier.KnockbackStrengthPercent);
        DashSpeed *= PercentToMult(modifier.DashDistancePercent);
    }

    public float PercentToMult(float percent) {
        float mult = 1 + (percent / 100);
        return mult;
    }
}