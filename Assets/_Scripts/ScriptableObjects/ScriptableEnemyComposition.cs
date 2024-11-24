using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyComposition", menuName = "Room Generation/Enemy Composition")]
public class ScriptableEnemyComposition : ScriptableObject {

    [SerializeField] private EnemyAmount[] initialEnemyAmounts;
    public EnemyAmount[] InitialEnemyAmounts => initialEnemyAmounts;

    [SerializeField] private bool spawnTimedEnemies;
    public bool SpawnTimedEnemies => spawnTimedEnemies;

    [ConditionalHide("spawnTimedEnemies")]
    [SerializeField] private float afterInitialEnemiesDelay;
    public float AfterInitialEnemiesDelay => afterInitialEnemiesDelay;

    [SerializeField] private EnemyAmount[] timedEnemyAmounts;
    public EnemyAmount[] TimedEnemyAmounts => timedEnemyAmounts;

    [ConditionalHide("spawnTimedEnemies")]
    [SerializeField] private RandomFloat betweenEnemyDelay;
    public RandomFloat BetweenEnemyDelay => betweenEnemyDelay;
}

[Serializable]
public struct EnemyAmount {
    public ScriptableEnemy ScriptableEnemy;
    public RandomInt Amount;
}
