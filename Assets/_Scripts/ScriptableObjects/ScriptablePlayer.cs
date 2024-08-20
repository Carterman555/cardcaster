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
}

[Serializable]
public class StatsBoost {

    public float DamagePercent;
    public float ReloadSpeedPercent;
    public float BulletSpeedPercent;
    public int BulletCountIncrease;
    public float HealthPercent;

    public static StatsBoost operator +(StatsBoost a, StatsBoost b) {
        return new StatsBoost {
            DamagePercent = a.DamagePercent + b.DamagePercent,
            ReloadSpeedPercent = a.ReloadSpeedPercent + b.ReloadSpeedPercent,
            BulletSpeedPercent = a.BulletSpeedPercent + b.BulletSpeedPercent,
            HealthPercent = a.HealthPercent + b.HealthPercent,
            BulletCountIncrease = a.BulletCountIncrease + b.BulletCountIncrease,
        };
    }

    public static StatsBoost Zero = new StatsBoost {
        DamagePercent = 0,
        ReloadSpeedPercent = 0,
        BulletSpeedPercent = 0,
        BulletCountIncrease = 0,
        HealthPercent = 0,
    };
}