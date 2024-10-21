using System;
using System.Collections;
using System.Collections.Generic;
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
public class EnemyStats : Stats {
    public float WalkSpeed;
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
