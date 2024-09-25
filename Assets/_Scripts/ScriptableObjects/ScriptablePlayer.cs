using System;
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

    public float AttackCooldown => 1 / AttackSpeed;
}

[Serializable]
public class PlayerStats : Stats {
    public float SwordSize;

    public float DashSpeed;
    public float DashTime;

    public void ApplyModifier(PlayerStatsModifier modifier) {
        MaxHealth *= modifier.MaxHealthPercent.PercentToMult();
        KnockbackResistance *= modifier.KnockbackResistancePercent.PercentToMult();
        MoveSpeed *= modifier.MoveSpeedPercent.PercentToMult();
        Damage *= modifier.DamageIncreasePercent.PercentToMult();
        AttackSpeed *= modifier.AttackSpeedPercent.PercentToMult();
        KnockbackStrength *= modifier.KnockbackStrengthPercent.PercentToMult();
        SwordSize *= modifier.SwordSizePercent.PercentToMult();
        DashSpeed *= modifier.DashDistancePercent.PercentToMult();
    }
}