using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Unit/Enemy")]
public class ScriptableEnemy : ScriptableObject {

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    [field: SerializeField] public Enemy Prefab { get; private set; }

    [field: SerializeField] public EnemyStats Stats { get; private set; }
    [field: SerializeField] public float Difficulty { get; private set; }

    [field: SerializeField] public EnemyTag Tags { get; private set; }
}

[Serializable]
public struct EnemyStats {
    public float MaxHealth;
    public float KnockbackResistance;

    public float MoveSpeed;

    public float Damage;
    public float AttackSpeed;
    public float AttackCooldown => 1 / AttackSpeed;
    public float KnockbackStrength;
    public float AttackRange;
}

[Flags]
public enum EnemyTag {
    None = 0,
    BasicMelee = 1 << 0,
    BasicRanged = 1 << 1,
    CircleShoot = 1 << 2,
    Tank = 1 << 3
}
