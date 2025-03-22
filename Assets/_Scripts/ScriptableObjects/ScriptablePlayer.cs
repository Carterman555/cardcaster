using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Unit/Player")]
public class ScriptablePlayer : ScriptableObject {

    [field: SerializeField] public PlayerStats BaseStats { get; private set; }
}

[Serializable]
public struct CommonStats {
    public float MaxHealth;
    public float KnockbackResistance;

    public float MoveSpeed;

    public float Damage;
    public float AttackSpeed;
    public float AttackCooldown => 1 / AttackSpeed;
    public float KnockbackStrength;
}

[Serializable]
public struct PlayerStats {

    public CommonStats CommonStats;

    public float SwordSize;

    public float DashSpeed;
    public float DashDistance;
    public float DashAttackDamage;
    public float DashRechargeSpeed;
    public float DashCooldown => 1 / DashRechargeSpeed;

    public float CritChance;
    public float CritDamageMult;

    public float ProjectileDamageMult;
    public float AllDamageMult;

    public int MaxEssence;
    public int HandSize;
}

[Serializable]
public struct PlayerStatsModifier {
    public PlayerStatModifier[] StatModifiers;
    public string ID;
}

[Serializable]
public struct PlayerStatModifier {
    public PlayerStatType PlayerStatType;
    public ModifyType ModifyType;
    public float Value;
}

public enum PlayerStatType {
    MaxHealth,
    KnockbackResistance,

    MoveSpeed,

    Damage,
    AttackSpeed,
    KnockbackStrength,

    SwordSize,

    DashSpeed,
    DashTime,
    DashAttackDamage,
    DashRechargeSpeed,

    CritChance,
    CritDamageMult,

    ProjectileDamageMult,
    AllDamageMult,

    MaxEssence,
    HandSize
}

public enum ModifyType {
    Additive,
    Multiplicative
}