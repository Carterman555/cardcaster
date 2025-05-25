using System;
using UnityEngine;

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

    public float BaseBasicAttackDamage;
    public float AttackSpeed;
    public float AttackCooldown => 1 / AttackSpeed;
    public float KnockbackStrength;
    public float SwordSize;

    public float DashDistance;
    public float BaseDashAttackDamage;
    public float DashRechargeSpeed;
    public float DashCooldown => 1 / DashRechargeSpeed;

    public float CritChance;
    public float CritDamageMult;

    public float BaseProjectileDamageMult;
    public float AllDamageMult;

    public float BasicAttackDamage => BaseBasicAttackDamage * AllDamageMult;
    public float DashAttackDamage => BaseDashAttackDamage * AllDamageMult;
    public float ProjectileDamageMult => BaseProjectileDamageMult * AllDamageMult;

    public int MaxEssence;
    public int HandSize;
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

    DashDistance,
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