using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Unit/Enemy")]
public class ScriptableEnemy : ScriptableObject {

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    [field: SerializeField] public Enemy Prefab { get; private set; }

    [field: SerializeField] public EnemyStats Stats { get; private set; }
}

[Serializable]
public class EnemyStats : Stats {
    public float WalkSpeed;
    public float AttackDistance;
}
