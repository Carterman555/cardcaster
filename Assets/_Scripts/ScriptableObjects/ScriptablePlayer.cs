using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Unit/Player")]
public class ScriptablePlayer : ScriptableObject {
    [SerializeField] private PlayerStats baseStats;
    public PlayerStats PlayerStats => baseStats;
}

[Serializable]
public struct PlayerStats {
    public float MaxHealth;
    public float KnockbackResistance;

    public float MoveSpeed;

    public float Damage;
    public float AttackSpeed;
    public float AttackCooldown => 1 / AttackSpeed;
    public float KnockbackStrength;
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
public class PlayerStatsModifier {
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