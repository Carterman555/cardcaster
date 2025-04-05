using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyComposition", menuName = "Room Generation/Enemy Composition")]
public class ScriptableEnemyComposition : ScriptableObject {

    [SerializeField] private EnemyWave[] enemyWaves;
    public EnemyWave[] EnemyWaves => enemyWaves;
}

[Serializable]
public struct EnemyAmount {
    public ScriptableEnemy ScriptableEnemy;
    public RandomInt Amount;
}

[Serializable]
public struct EnemyWave {
    public EnemyAmount[] EnemyAmounts;
}
